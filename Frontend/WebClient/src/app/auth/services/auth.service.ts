import {AuthSetup} from '../auth-setup';
import {HttpClient, HttpContext, HttpContextToken, HttpHeaders,} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {
  AuthorizationError,
  AuthorizationNotifier,
  AuthorizationRequest,
  AuthorizationResponse,
  AuthorizationServiceConfiguration,
  BaseTokenRequestHandler,
  BasicQueryStringUtils,
  FetchRequestor,
  GRANT_TYPE_AUTHORIZATION_CODE,
  GRANT_TYPE_REFRESH_TOKEN,
  LocalStorageBackend,
  LocationLike,
  RedirectRequestHandler,
  RevokeTokenRequest,
  StringMap,
  TokenRequest,
  TokenRequestHandler,
} from '@openid/appauth';
import {BehaviorSubject, from, lastValueFrom, map} from 'rxjs';
import {environment} from 'src/environments/environment';
import {AuthInfo} from '../models/auth-info';

type AuthorizationCallback = (authInfo: AuthInfo | null) => void;

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private tokenHandler!: TokenRequestHandler;
  private authorizationHandler!: RedirectRequestHandler;
  private notifier!: AuthorizationNotifier;
  private configuration?: AuthorizationServiceConfiguration;

  private signinRedirectUrl!: string;
  private signoutRedirectUrl!: string;

  private taskCount = 0;
  private isLoadingSubject = new BehaviorSubject<boolean>(false);

  private authInfoSubject = new BehaviorSubject<AuthInfo | null>(null);

  private setup: AuthSetup = environment.authSetup;

  private authCallback?: AuthorizationCallback;

  private refreshTimer?: NodeJS.Timeout;

  private refreshTokenPromise?: Promise<any>;

  isLoading$ = this.isLoadingSubject.asObservable();
  authInfo$ = this.authInfoSubject.asObservable();

  isAuthenticated$ = this.authInfoSubject.pipe(
    map((userInfo) => userInfo != null)
  );

  constructor(
    private httpClient: HttpClient,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.initializeUrls();
    this.initializeHandlers();
  }

  restoreAuthInfo() {
    const authInfoJson = localStorage.getItem('auth_info');

    if (authInfoJson) {
      const authInfo = JSON.parse(authInfoJson) as AuthInfo;

      const refreshTokenExpireTime =
        (authInfo.token.issued_at! + this.setup.refreshTokenLifetime - 60) *
        1000;

      if (refreshTokenExpireTime > Date.now()) {
        this.setAuthInfo(authInfo, false);
        return true;
      } else {
        localStorage.removeItem('auth_info');
      }
    }

    return false;
  }

  private setAuthInfo(
    authInfo: AuthInfo | null,
    store: boolean,
    storeOnly: boolean = false
  ) {
    if (store) {
      if (authInfo) {
        localStorage.setItem('auth_info', JSON.stringify(authInfo));
      } else {
        localStorage.removeItem('auth_info');
      }
    }

    if (!storeOnly) {
      this.authInfoSubject.next(authInfo);
      this.setupRefreshTokenTimer(authInfo);
    }
  }

  private setupRefreshTokenTimer(authInfo: AuthInfo | null) {
    if (this.refreshTimer) {
      clearTimeout(this.refreshTimer);
      this.refreshTimer = undefined;
    }

    if (authInfo && this.setup.autoRefreshToken) {
      const time =
        (authInfo.token.issued_at! + Number(authInfo.token.expires_in) - 60) *
          1000 -
        Date.now();

      this.refreshTimer = setTimeout(() => {
        this.doRefreshToken(false);
      }, time);
    }
  }

  private initializeHandlers() {
    this.tokenHandler = new BaseTokenRequestHandler(new FetchRequestor());

    // Create the authorization notifier
    this.notifier = new AuthorizationNotifier();
    this.notifier.setAuthorizationListener(
      this.handleAuthorizationResponse.bind(this)
    );

    // Create the authorization request handler
    this.authorizationHandler = new RedirectRequestHandler(
      new LocalStorageBackend(),
      new NoHashQueryStringUtils()
    );
    this.authorizationHandler.setAuthorizationNotifier(this.notifier);
  }

  private initializeUrls() {
    this.signinRedirectUrl =
      location.origin +
      this.router.createUrlTree(['signin-callback']).toString();

    this.signoutRedirectUrl =
      location.origin +
      this.router.createUrlTree(['signout-callback']).toString();
  }

  private startTask() {
    if (this.taskCount === 0) {
      this.isLoadingSubject.next(true);
    }

    this.taskCount++;
  }

  private endTask() {
    this.taskCount = Math.max(0, this.taskCount - 1);

    if (this.taskCount === 0) {
      this.isLoadingSubject.next(false);
    }
  }

  private async initialize() {
    if (!!this.configuration) return;

    this.configuration =
      await AuthorizationServiceConfiguration.fetchFromIssuer(
        this.setup.idpUrl,
        new FetchRequestor()
      );
  }

  private async handleAuthorizationResponse(
    request: AuthorizationRequest,
    response: AuthorizationResponse | null,
    error: AuthorizationError | null
  ) {
    console.log('Handling authorization response', request, response, error);

    const authCallback = this.authCallback;
    this.authCallback = undefined;

    let authInfo: AuthInfo | null = null;

    if (response) {
      this.startTask();

      console.log(`Authorization Code  ${response.code}`);

      try {
        // pkce workflow requires sending the code_verifier to the idp for validation
        let extras: any = {};
        if (request && request.internal) {
          extras['code_verifier'] = request.internal['code_verifier'];
        }

        // Create a request for exchanging the access token with the authorization code
        const tokenRequest = new TokenRequest({
          client_id: this.setup.clientId,
          redirect_uri: this.signinRedirectUrl,
          grant_type: GRANT_TYPE_AUTHORIZATION_CODE,
          code: response.code,
          extras,
        });

        authInfo = await this.requestTokenAndUserInfo(tokenRequest);
      } catch (err) {
        console.error('Failed to handle authorization response', err);
      }

      this.endTask();
    }

    if (authCallback) {
      authCallback(authInfo);
    }
  }

  // Call this method to verify the authorization code responded from idp
  validateAuthorizationCode() {
    return new Promise<AuthInfo>((resolve, reject) => {
      this.doValidateAuthorizationCode((authInfo) => {
        if (authInfo) {
          resolve(authInfo);
        } else {
          reject(new Error('Failed to validate authorization code'));
        }
      });
    });
  }

  private async doValidateAuthorizationCode(callback: AuthorizationCallback) {
    if (this.isLoadingSubject.getValue()) {
      throw new Error('AuthService is busy');
    }

    this.authCallback = (authInfo) => {
      if (!authInfo) {
        this.setAuthInfo(null, true);
      }
      callback(authInfo);
    };

    if (this.route.snapshot.queryParams['code']) {
      this.startTask();
      await this.authorizationHandler.completeAuthorizationRequestIfPossible();
      this.endTask();
    }

    if (this.authCallback) {
      this.authCallback(null);
      this.authCallback = undefined;
    }
  }

  // Call this method to request token refresh
  private async doRefreshToken(signoutOnFailure: boolean = true) {
    if (this.refreshTokenPromise) {
      return await this.refreshTokenPromise;
    }

    const authInfoJson = localStorage.getItem('auth_info');

    let authInfo: AuthInfo | null;

    if (authInfoJson) {
      authInfo = JSON.parse(authInfoJson) as AuthInfo;
    } else {
      authInfo = this.authInfoSubject.getValue();
    }

    if (!authInfo) {
      throw new Error('You are not signed in');
    }

    console.log('Refreshing token');

    // Create a request for exchanging the access token with the refresh token
    const tokenRequest = new TokenRequest({
      client_id: this.setup.clientId,
      redirect_uri: this.signinRedirectUrl,
      grant_type: GRANT_TYPE_REFRESH_TOKEN,
      refresh_token: authInfo.token.refresh_token!,
    });

    try {
      this.refreshTokenPromise = this.requestTokenAndUserInfo(
        tokenRequest
      ) as Promise<any>;

      await this.refreshTokenPromise;

      this.refreshTokenPromise = undefined;
    } catch (error: any) {
      this.refreshTokenPromise = undefined;

      console.error('Failed to refresh token', error);

      if (signoutOnFailure) {
        const statusCode = this.getStatusCode(error);

        if (this.shouldSignout(statusCode)) {
          console.warn('Logged out due to failure of refreshing token');
          this.setAuthInfo(null, true);
        }
      }

      throw error;
    }
  }

  private getStatusCode(error: any) {
    return Number(error.message);
  }

  private shouldSignout(statusCode: number) {
    // return statusCode >= 400 && statusCode < 500;
    return statusCode == 400;
  }

  private async requestTokenAndUserInfo(tokenRequest: TokenRequest) {
    // Ensure the service is initialized
    await this.initialize();

    const tokenResponse = await this.tokenHandler.performTokenRequest(
      this.configuration!,
      tokenRequest
    );

    const token = tokenResponse.toJson();

    let oldAuthInfo = this.authInfoSubject.value;
    if (oldAuthInfo != null) {
      oldAuthInfo.token = token;
      this.setAuthInfo(oldAuthInfo, true, true);
    }

    const userInfo: any = await lastValueFrom(
      this.httpClient.get(this.configuration!.userInfoEndpoint!, {
        headers: new HttpHeaders({
          Authorization: `Bearer ${tokenResponse.accessToken}`,
        }),
        context: this.AuthServiceRequest(),
      })
    );

    const authInfo: AuthInfo = {
      sub: userInfo.sub,
      email: userInfo.email,
      name: userInfo.name,
      role: Array.isArray(userInfo.role) ? userInfo.role : [userInfo.role],
      token: token,
    };

    this.setAuthInfo(authInfo, true);

    console.log('Token refreshed', authInfo);

    return authInfo;
  }

  private async doSigninRedirect() {
    this.startTask();

    try {
      // Ensure the service is initialized
      await this.initialize();

      // Create an authentication request
      let request = new AuthorizationRequest({
        client_id: this.setup.clientId,
        redirect_uri: this.signinRedirectUrl,
        scope: this.setup.scope,
        response_type: AuthorizationRequest.RESPONSE_TYPE_CODE,
      });

      // Make the authentication request
      this.authorizationHandler.performAuthorizationRequest(
        this.configuration!,
        request
      );
    } catch (error) {
      console.error('Failed to sign in');
      throw error;
    }

    this.endTask();
  }

  private async doSignoutRedirect() {
    const authInfo = this.authInfoSubject.getValue();
    if (!authInfo) {
      throw new Error('You are not signed in');
    }

    this.startTask();

    try {
      // Ensure the service is initialized
      await this.initialize();

      const request = new RevokeTokenRequest({
        client_id: this.setup.clientId,
        token: authInfo.token.refresh_token!,
      });

      await this.tokenHandler.performRevokeTokenRequest(
        this.configuration!,
        request
      );

      const idTokenHint = authInfo.token.id_token!;
      const redirect_uri = this.signoutRedirectUrl;

      const logoutReqURL =
        this.configuration!.endSessionEndpoint +
        `?client_id=${this.setup.clientId}&id_token_hint=${idTokenHint}&post_logout_redirect_uri=${redirect_uri}`;

      localStorage.removeItem('auth_info');
      window.location.href = logoutReqURL;

      this.endTask();
    } catch (error) {
      console.error('Failed to sign out', error);
      throw error;
    }
  }

  // Call this method to clear auth info when the user is unauthorized
  clearAuthInfo() {
    this.setAuthInfo(null, true);
  }

  refreshToken(signoutOnFailure: boolean = true) {
    return from(this.doRefreshToken(signoutOnFailure));
  }

  signinRedirect() {
    return from(this.doSigninRedirect());
  }

  signoutRedirect() {
    return from(this.doSignoutRedirect());
  }

  AuthServiceRequest() {
    return new HttpContext().set(IS_AUTH_SERVICE_REQUEST, true);
  }
}

class NoHashQueryStringUtils extends BasicQueryStringUtils {
  override parse(input: LocationLike, useHash?: boolean): StringMap {
    return super.parse(input, false /* never use hash */);
  }
}

export const IS_AUTH_SERVICE_REQUEST = new HttpContextToken<boolean>(
  () => false
);
