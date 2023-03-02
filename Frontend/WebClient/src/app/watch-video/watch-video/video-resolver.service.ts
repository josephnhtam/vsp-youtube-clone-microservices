import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  Resolve,
  Router,
  RouterStateSnapshot,
} from '@angular/router';
import { catchError, Observable, of } from 'rxjs';
import { Video } from '../../core/models/video';

@Injectable()
export class VideoResolver implements Resolve<Video | null> {
  constructor(private httpClient: HttpClient, private router: Router) {}

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<Video | null> {
    const id = route.params['id'];
    const url = environment.appSetup.apiUrl + `/api/v1/VideoStore/Videos/${id}`;

    return this.httpClient.get<Video>(url).pipe(
      catchError((error) => {
        console.error(error);

        if (
          error.status == 400 ||
          error.status == 401 ||
          error.status == 403 ||
          error.status == 404
        ) {
          this.router.navigate(['/', 'unavailable', 'video'], {
            skipLocationChange: true,
            state: {
              statusCode: error.status,
            },
          });
        } else {
          this.router.navigate(['/'], {
            skipLocationChange: true,
          });
        }

        return of(null);
      })
    );
  }
}
