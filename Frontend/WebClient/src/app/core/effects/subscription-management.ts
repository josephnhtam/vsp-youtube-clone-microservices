import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, concatMap, map, of, switchMap } from 'rxjs';
import { environment } from 'src/environments/environment';
import {
  SubscriptionManagementAction,
  SubscriptionManagementApiAction,
} from '../actions';
import {
  ChangeNotificationTypeRequest,
  SubscribeRequest,
  Subscriptions,
} from '../models/subscription';

@Injectable()
export class SubscriptionManagementEffects {
  constructor(private actions$: Actions, private httpClient: HttpClient) {}

  unsubscribeEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(SubscriptionManagementAction.unsubscribe),

      concatMap(({ userId }) => {
        const url = environment.appSetup.apiUrl + '/api/v1/Subscriptions';

        const params = new HttpParams({
          fromObject: {
            userId,
          },
        });

        return this.httpClient
          .delete<void>(url, {
            params,
          })
          .pipe(
            map(() => {
              return SubscriptionManagementApiAction.unsubscribed({
                userId,
              });
            }),

            catchError((error) => {
              this.showErrorMsg(error);
              return of(
                SubscriptionManagementApiAction.failedToUnsubscribe({
                  userId,
                })
              );
            })
          );
      })
    )
  );

  changeNotificationTypeEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(SubscriptionManagementAction.changeNotificationType),

      concatMap(({ userId, notificationType }) => {
        const url = environment.appSetup.apiUrl + '/api/v1/Subscriptions';

        const request: ChangeNotificationTypeRequest = {
          userId,
          notificationType,
        };

        return this.httpClient.put<void>(url, request).pipe(
          map(() => {
            return SubscriptionManagementApiAction.notificationTypeChanged({
              userId,
              notificationType,
            });
          }),

          catchError((error) => {
            this.showErrorMsg(error);
            return of(
              SubscriptionManagementApiAction.failedToChangeNotificationType({
                userId,
              })
            );
          })
        );
      })
    )
  );

  subscribeEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(SubscriptionManagementAction.subscribe),

      concatMap(({ userId, notificationType, userProfile }) => {
        const url = environment.appSetup.apiUrl + '/api/v1/Subscriptions';

        const request: SubscribeRequest = {
          userId,
          notificationType,
        };

        return this.httpClient.post<void>(url, request).pipe(
          map(() => {
            return SubscriptionManagementApiAction.subscribed({
              userId,
              notificationType,
              userProfile,
            });
          }),

          catchError((error) => {
            this.showErrorMsg(error);
            return of(
              SubscriptionManagementApiAction.failedToSubscribe({
                userId,
              })
            );
          })
        );
      })
    )
  );

  retrieveSubscriptionInfosEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(SubscriptionManagementAction.retrieveSubscriptions),

      switchMap(({ maxCount }) => {
        const url = environment.appSetup.apiUrl + '/api/v1/Subscriptions';

        let params = new HttpParams();
        if (!!maxCount) {
          params = params.append('page', 1);
          params = params.append('pageSize', maxCount);
        }

        return this.httpClient.get<Subscriptions>(url, { params }).pipe(
          map((result) => {
            return SubscriptionManagementApiAction.subscriptionsRetrieved({
              subscriptions: result.subscriptions,
            });
          }),

          catchError((error) => {
            this.showErrorMsg(error);

            return of(
              SubscriptionManagementApiAction.failedToRetrieveSubscriptions()
            );
          })
        );
      })
    )
  );

  private showErrorMsg(error: any) {
    console.error(error);
  }
}
