import {AuthService, IS_AUTH_SERVICE_REQUEST} from '../services/auth.service';
import {HttpEvent, HttpHandler, HttpInterceptor, HttpRequest,} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {BehaviorSubject, catchError, concatMap, filter, finalize, Observable, switchMap, take,} from 'rxjs';

@Injectable()
export class RefreshTokenInterceptor implements HttpInterceptor {
  private refreshingToken = false;
  private afterTokenRefreshedSubject: BehaviorSubject<boolean> =
    new BehaviorSubject<boolean>(false);

  constructor(private authService: AuthService) {}

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    if (req.context.get(IS_AUTH_SERVICE_REQUEST)) {
      return next.handle(req);
    }

    if (this.refreshingToken) {
      return this.afterTokenRefreshedSubject.pipe(
        filter((tokenRefreshComplete) => tokenRefreshComplete),
        take(1),
        switchMap(() => {
          return next.handle(req);
        })
      );
    }

    return this.authService.authInfo$.pipe(
      take(1),

      concatMap((authInfo) => {
        if (!authInfo) {
          return next.handle(req);
        }

        const currentTimestamp = Date.now();

        const accessTokenExpireTimestamp =
          (authInfo.token.issued_at! + Number(authInfo.token.expires_in) - 60) *
          1000;

        if (currentTimestamp > accessTokenExpireTimestamp) {
          this.refreshingToken = true;
          this.afterTokenRefreshedSubject.next(false);

          console.log(
            '[RefreshTokenInterceptor] Refreshing token due to access token expired'
          );

          return this.authService.refreshToken(true).pipe(
            switchMap(() => {
              console.log(
                '[RefreshTokenInterceptor] Refreshing token completed'
              );
              this.refreshingToken = false;

              return next.handle(req).pipe(
                finalize(() => {
                  this.afterTokenRefreshedSubject.next(true);
                })
              );
            }),

            catchError((error) => {
              console.log(
                '[RefreshTokenInterceptor] Refreshing token failed',
                error
              );
              this.refreshingToken = false;

              return next.handle(req).pipe(
                finalize(() => {
                  this.afterTokenRefreshedSubject.next(true);
                })
              );
            })
          );
        } else {
          return next.handle(req);
        }
      })
    );
  }
}
