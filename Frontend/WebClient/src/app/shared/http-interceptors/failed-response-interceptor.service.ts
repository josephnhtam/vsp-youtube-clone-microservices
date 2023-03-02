import {FailedResponse} from '../../core/models/failed-response';
import {
  HttpContext,
  HttpContextToken,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {catchError, Observable} from 'rxjs';
import {ErrorDialogService} from '../error-dialog/error-dialog.service';

@Injectable()
export class FailedResponseInterceptor implements HttpInterceptor {
  constructor(private service: ErrorDialogService) {}

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((error) => {
        const bypass = req.context.get(BYPASS_FAILED_RESPONSE);

        if (bypass.statusCodes.some((x) => x == error.status)) {
          throw error;
        }

        if (error.status == 0) {
          this.service.openErrorDialog(
            'Oops, something went wrong. Please try again.'
          );
        } else if (error.status == 408) {
          this.service.openErrorDialog('Request timeout. Please try again.');
        } else if (req.method !== 'GET') {
          const failedResponse = error.error as FailedResponse;

          if (!!failedResponse.message) {
            const message = failedResponse.message.trim();
            if (message != '') {
              this.service.openErrorDialog(message, 5);
            }
          }
        }

        throw error;
      })
    );
  }
}

export interface BypassFailedResponseContext {
  statusCodes: number[];
}

export const BYPASS_FAILED_RESPONSE =
  new HttpContextToken<BypassFailedResponseContext>(() => {
    return { statusCodes: [] };
  });

export function BypassFailedResponse(statusCodes: number[]) {
  return new HttpContext().set(BYPASS_FAILED_RESPONSE, { statusCodes });
}
