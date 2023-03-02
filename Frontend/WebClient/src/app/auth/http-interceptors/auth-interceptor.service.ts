import {AuthService} from '../services/auth.service';
import {HttpEvent, HttpHandler, HttpHeaders, HttpInterceptor, HttpRequest,} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {concatMap, Observable, take} from 'rxjs';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(private authService: AuthService) {}

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    return this.authService.authInfo$.pipe(
      take(1),
      concatMap((authInfo) => {
        if (!req.headers.has('Authorization')) {
          if (authInfo != null) {
            // console.log('AuthInterceptor: adding bearer token');
            req = req.clone({
              headers: new HttpHeaders({
                Authorization: `Bearer ${authInfo.token.access_token}`,
              }),
            });
          }
        }

        try {
          return next.handle(req);
        } catch (error) {
          console.error(error);
          throw error;
        }
      })
    );
  }
}
