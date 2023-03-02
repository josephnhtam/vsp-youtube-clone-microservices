import {map, of, switchMap} from 'rxjs';
import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot,} from '@angular/router';
import {Store} from '@ngrx/store';
import {AuthService} from 'src/app/auth/services/auth.service';
import {selectUserProfileClient} from '../../core/selectors/users';
import {UserProfileStatus} from '../../core/models/users';

@Injectable({
  providedIn: 'root',
})
export class UserReadyGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private store: Store,
    private router: Router
  ) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    return this.authService.authInfo$.pipe(
      switchMap((authInfo) => {
        if (!authInfo) {
          return of(true);
        }

        return this.store.select(selectUserProfileClient(authInfo.sub)).pipe(
          map((client) => {
            if (!client) return true;

            if (client.userProfile?.status === UserProfileStatus.Registered) {
              return true;
            } else {
              return this.router.createUrlTree(['/']);
            }
          })
        );
      })
    );
  }
}
