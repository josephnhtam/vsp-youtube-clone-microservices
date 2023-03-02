import { UserHistorySettingsService } from './user-history-settings.service';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  Router,
  RouterStateSnapshot,
} from '@angular/router';
import { catchError, map, of } from 'rxjs';
import { UserHistorySettings } from '../../core/models/history';
import { environment } from 'src/environments/environment';

@Injectable()
export class HistoryGuard implements CanActivate {
  constructor(
    private service: UserHistorySettingsService,
    private httpClient: HttpClient,
    private router: Router
  ) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    const url = environment.appSetup.apiUrl + '/api/v1/UserHistory/Settings';

    return this.httpClient.get<UserHistorySettings>(url).pipe(
      map((settings) => {
        this.service.updateSettings(settings);
        return true;
      }),

      catchError((error) => {
        console.error(error);
        return of(this.router.createUrlTree(['/']));
      })
    );
  }
}
