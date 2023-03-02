import { AuthInfo } from '../../auth/models/auth-info';
import { UsersAction, UsersApiAction } from 'src/app/core/actions';
import { AuthService } from '../../auth/services/auth.service';
import { Store } from '@ngrx/store';
import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  RouterStateSnapshot,
  UrlTree,
} from '@angular/router';
import { Observable, of, switchMap, take } from 'rxjs';
import { Actions, ofType } from '@ngrx/effects';
import { CreateUserProfileDialogService } from '../create-user-profile-dialog/create-user-profile-dialog.service';

@Injectable()
export class UserProfileResolver implements CanActivate {
  constructor(
    private store: Store,
    private authService: AuthService,
    private actions$: Actions,
    private dialog: CreateUserProfileDialogService
  ) {}

  getUserProfile(): Observable<boolean | UrlTree> {
    return this.authService.authInfo$.pipe(
      take(1),

      switchMap((authInfo) => {
        if (!authInfo) {
          return of(true);
        }

        this.store.dispatch(
          UsersAction.getUserProfile({ userId: authInfo.sub })
        );

        return this.actions$.pipe(
          ofType(
            UsersApiAction.userProfilesObtained,
            UsersApiAction.failedToObtainUserProfiles
          ),

          switchMap((action) => {
            if (action.type === UsersApiAction.userProfilesObtained.type) {
              return of(true);
            }

            if (action.error.status === 404) {
              // User profile not created
              return this.createUserProfile(authInfo);
            } else {
              // Maybe the user profile manager service is down.
              // Let user continue without user profile
              return of(true);
            }
          })
        );
      })
    );
  }

  createUserProfile(authInfo: AuthInfo): Observable<boolean | UrlTree> {
    this.dialog.openCreateUserProfileDialog();
    return of(true);
  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    return this.getUserProfile();
  }
}
