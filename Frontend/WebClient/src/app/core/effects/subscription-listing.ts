import { HttpClient, HttpParams } from '@angular/common/http';
import { Store } from '@ngrx/store';
import { Injectable } from '@angular/core';
import { Actions, concatLatestFrom, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, mergeMap, of } from 'rxjs';
import {
  SubscriptionListingAction,
  SubscriptionListingApiAction,
} from '../actions';
import { environment } from 'src/environments/environment';
import { selectSubscriptionListingState } from '../selectors';
import { DetailedSubscriptions } from '../models/subscription';

@Injectable()
export class SubscriptionListingEffects {
  constructor(
    private actions$: Actions,
    private store: Store,
    private httpClient: HttpClient
  ) {}

  getSubscriptionsEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(SubscriptionListingAction.getMoreSubscriptions),

      concatLatestFrom(() => this.store.select(selectSubscriptionListingState)),

      mergeMap(([action, state]) => {
        const url =
          environment.appSetup.apiUrl + '/api/v1/Subscriptions/Detailed';

        const page = state.currentPage + 1;

        const params = new HttpParams({
          fromObject: {
            page,
            pageSize: state.pageSize,
            sort: state.sort,
          },
        });

        return this.httpClient.get<DetailedSubscriptions>(url, { params }).pipe(
          map((result) => {
            return SubscriptionListingApiAction.moreSubscriptionsObtained({
              contextId: state.contextId,
              page,
              subscriptions: result.subscriptions.map((dto) => {
                return {
                  ...dto,
                  isSubscribed: true,
                };
              }),
            });
          }),

          catchError((error) => {
            console.error(error);

            return of(
              SubscriptionListingApiAction.failedToObtainMoreSubscriptions({
                contextId: state.contextId,
                error,
              })
            );
          })
        );
      })
    )
  );

  getUserSubscriptionsEFfect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(SubscriptionListingAction.getSubscriptions),

      concatLatestFrom(() => this.store.select(selectSubscriptionListingState)),

      mergeMap(([{ pageSize, sort }, state]) => {
        const url =
          environment.appSetup.apiUrl + '/api/v1/Subscriptions/Detailed';

        const params = new HttpParams({
          fromObject: {
            page: 1,
            pageSize,
            sort,
            includeTotalCount: true,
          },
        });

        return this.httpClient
          .get<DetailedSubscriptions>(url, {
            params,
          })
          .pipe(
            map((result) => {
              return SubscriptionListingApiAction.subscriptionsObtained({
                contextId: state.contextId,
                pageSize,
                sort,
                subscriptionsCount: result.totalCount,
                subscriptions: result.subscriptions.map((dto) => {
                  return {
                    ...dto,
                    isSubscribed: true,
                  };
                }),
              });
            }),

            catchError((error) => {
              console.error(error);

              return of(
                SubscriptionListingApiAction.failedToObtainSubscriptions({
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
