import { Store } from '@ngrx/store';
import { AuthService } from 'src/app/auth/services/auth.service';
import { UserProfile, UserProfileStatus } from '../models/users';
import { environment } from 'src/environments/environment';
import { Actions, concatLatestFrom, createEffect, ofType } from '@ngrx/effects';
import { HttpClient, HttpParams } from '@angular/common/http';
import { UsersAction, UsersApiAction } from '../actions';
import { catchError, concatMap, filter, map, mergeMap, of } from 'rxjs';
import { Injectable } from '@angular/core';

@Injectable()
export class UserProfilesEffect {
  constructor(
    private actions$: Actions,
    private authService: AuthService,
    private httpClient: HttpClient,
    private store: Store
  ) {}

  createUserEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(UsersAction.createUser),

      concatLatestFrom(() => this.authService.authInfo$),

      filter(([_, authInfo]) => authInfo != null),

      concatMap(([{ displayName, thumbnailToken }, authInfo]) => {
        const userId = authInfo!.sub;

        const request = {
          displayName,
          thumbnailToken,
        };

        const url = environment.appSetup.apiUrl + '/api/v1/Users/';

        return this.httpClient.post(url, request).pipe(
          map(() => {
            return UsersApiAction.userCreated({ userId });
          }),

          catchError((error) => {
            return of(
              UsersApiAction.failedToCreateUser({
                userId,
                error,
              })
            );
          })
        );
      })
    )
  );

  getUserProfileEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(UsersAction.getUserProfile),

      concatLatestFrom(() => this.authService.authInfo$),

      mergeMap(([{ userId }, authInfo]) => {
        let url: string;
        let params = new HttpParams();

        if (userId !== authInfo?.sub) {
          url = environment.appSetup.apiUrl + '/api/v1/UserProfiles/';
          params = params.append('userId', userId);
        } else {
          url = environment.appSetup.apiUrl + '/api/v1/UserProfiles/Self';
        }

        return this.httpClient.get<UserProfile>(url).pipe(
          map((userProfile) => {
            if (
              userProfile.status != null &&
              userProfile.status == UserProfileStatus.Created &&
              userProfile.id === authInfo?.sub
            ) {
              setTimeout(
                () =>
                  this.store.dispatch(UsersAction.getUserProfile({ userId })),
                3000
              );
            }

            return UsersApiAction.userProfilesObtained({
              userProfile,
            });
          }),

          catchError((error) => {
            return of(
              UsersApiAction.failedToObtainUserProfiles({
                userId,
                error,
              })
            );
          })
        );
      })
    )
  );
}
