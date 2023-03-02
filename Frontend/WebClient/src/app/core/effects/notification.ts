import { GetNotificationMessageResponse } from '../models/notification';
import { Store } from '@ngrx/store';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Actions, concatLatestFrom, createEffect, ofType } from '@ngrx/effects';
import { Injectable } from '@angular/core';
import { NotificationAction, NotificationApiAction } from '../actions';
import { catchError, concatMap, map, of } from 'rxjs';
import { environment } from 'src/environments/environment';
import { selectNotificationState } from '../selectors';

@Injectable()
export class NotificationEffect {
  constructor(
    private actions$: Actions,
    private httpClient: HttpClient,
    private store: Store
  ) {}

  markMessageAsCheckedEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(NotificationAction.markMessageAsChecked),

      concatMap(({ messageId }) => {
        const url =
          environment.appSetup.apiUrl + `/api/v1/Notifications/${messageId}`;

        return this.httpClient.put(url, null).pipe(
          map(() => {
            return NotificationApiAction.messageMarkedAsChecked({
              messageId,
            });
          }),

          catchError((error) => {
            return of(
              NotificationApiAction.failedToMarkMessageAsChecked({
                messageId,
                error,
              })
            );
          })
        );
      })
    )
  );

  removeMessageEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(NotificationAction.removeMessage),

      concatMap(({ messageId }) => {
        const url =
          environment.appSetup.apiUrl + `/api/v1/Notifications/${messageId}`;

        return this.httpClient.delete(url).pipe(
          map(() => {
            return NotificationApiAction.messageRemoved({
              messageId,
            });
          }),

          catchError((error) => {
            return of(
              NotificationApiAction.failedToRemoveMessage({
                error,
              })
            );
          })
        );
      })
    )
  );

  loadMoreMessagesEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(NotificationAction.loadMoreMessages),

      concatLatestFrom(() => this.store.select(selectNotificationState)),

      concatMap(([_, state]) => {
        const page = state.currentPage + 1;
        const pageSize = state.pageSize;

        const params = new HttpParams({
          fromObject: {
            page,
            pageSize,
          },
        });

        const url = environment.appSetup.apiUrl + '/api/v1/Notifications';

        return this.httpClient
          .get<GetNotificationMessageResponse>(url, {
            params,
          })
          .pipe(
            map((response) => {
              return NotificationApiAction.messagesObtained({
                contextId: state.contextId,
                totalCount: response.totalCount,
                messages: response.messages,
                page: page,
              });
            }),

            catchError((error) => {
              return of(
                NotificationApiAction.failedToObtainMessages({
                  contextId: state.contextId,
                  error,
                })
              );
            })
          );
      })
    )
  );

  loadMessagesEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(NotificationAction.loadMessages),

      concatLatestFrom(() => this.store.select(selectNotificationState)),

      concatMap(([{ pageSize }, state]) => {
        const params = new HttpParams({
          fromObject: {
            page: 1,
            pageSize,
          },
        });

        const url = environment.appSetup.apiUrl + '/api/v1/Notifications';

        return this.httpClient
          .get<GetNotificationMessageResponse>(url, {
            params,
          })
          .pipe(
            map((response) => {
              return NotificationApiAction.messagesObtained({
                contextId: state.contextId,
                totalCount: response.totalCount,
                messages: response.messages,
                page: 1,
              });
            }),

            catchError((error) => {
              return of(
                NotificationApiAction.failedToObtainMessages({
                  contextId: state.contextId,
                  error,
                })
              );
            })
          );
      })
    )
  );
}
