import { Injectable } from '@angular/core';
import { Actions, concatLatestFrom, createEffect, ofType } from '@ngrx/effects';
import { Store } from '@ngrx/store';
import { catchError, concatMap, map, Observable, of } from 'rxjs';
import { HistoryAction, HistoryApiAction } from '../actions';
import {
  SearchResponse,
  SearchUserWatchHistoryRequest,
} from '../models/history';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { selectHistoryState } from '../selectors';

@Injectable()
export class HistoryEffects {
  constructor(
    private actions$: Actions,
    private httpClient: HttpClient,
    private store: Store
  ) {}

  removeVideoEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(HistoryAction.removeVideo),

      concatLatestFrom(() => this.store.select(selectHistoryState)),

      concatMap(([{ videoId }, state]) => {
        const url = environment.appSetup.apiUrl + '/api/v1/UserHistory/Video';

        const params = new HttpParams({
          fromObject: {
            videoId,
          },
        });

        return this.httpClient
          .delete(url, {
            params,
          })
          .pipe(
            map(() =>
              HistoryApiAction.videoRemoved({
                contextId: state.contextId,
                videoId,
              })
            ),

            catchError((error) => {
              console.error(error);
              return of(
                HistoryApiAction.failedToRemoveVideo({
                  contextId: state.contextId,
                  error,
                })
              );
            })
          );
      })
    )
  );

  clearHistoryEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(HistoryAction.clearHistory),

      concatLatestFrom(() => this.store.select(selectHistoryState)),

      concatMap(([_, state]) => {
        const url = environment.appSetup.apiUrl + '/api/v1/UserHistory';

        return this.httpClient.delete(url).pipe(
          map(() =>
            HistoryApiAction.historyCleared({
              contextId: state.contextId,
            })
          ),

          catchError((error) => {
            console.error(error);
            return of(
              HistoryApiAction.failedToClearHistory({
                contextId: state.contextId,
                error,
              })
            );
          })
        );
      })
    )
  );

  loadMoreRecordsEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(HistoryAction.loadMoreRecords),

      concatLatestFrom(() => this.store.select(selectHistoryState)),

      concatMap(([_, state]) => {
        let search: Observable<SearchResponse>;

        const page = state.currentPage + 1;

        const params = new HttpParams({
          fromObject: this.createHttpParams(
            state.searchRequest!,
            page,
            state.pageSize
          ),
        });

        const url = environment.appSetup.apiUrl + '/api/v1/UserHistory';

        search = this.httpClient.get<SearchResponse>(url, {
          params,
        });

        return search.pipe(
          map((response) => {
            return HistoryApiAction.historyRecordsObtained({
              contextId: state.contextId,
              items: response.items,
              totalCount: response.totalCount,
              page,
            });
          }),

          catchError((error) => {
            console.error(error);
            return of(
              HistoryApiAction.failedToObtainHistoryRecord({
                contextId: state.contextId,
                error,
              })
            );
          })
        );
      })
    )
  );

  searchEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(HistoryAction.search),

      concatLatestFrom(() => this.store.select(selectHistoryState)),

      concatMap(([{ searchRequest, pageSize }, state]) => {
        let search: Observable<SearchResponse>;

        const params = new HttpParams({
          fromObject: this.createHttpParams(searchRequest, 1, pageSize),
        });

        const url = environment.appSetup.apiUrl + '/api/v1/UserHistory';

        search = this.httpClient.get<SearchResponse>(url, {
          params,
        });

        return search.pipe(
          map((response) => {
            return HistoryApiAction.historyRecordsObtained({
              contextId: state.contextId,
              items: response.items,
              totalCount: response.totalCount,
              page: 1,
            });
          }),

          catchError((error) => {
            console.error(error);
            return of(
              HistoryApiAction.failedToObtainHistoryRecord({
                contextId: state.contextId,
                error,
              })
            );
          })
        );
      })
    )
  );

  createHttpParams(
    searchRequest: SearchUserWatchHistoryRequest,
    page: number,
    pageSize: number
  ) {
    let params: any = {
      pageSize,
      page,
    };

    if (!!searchRequest.query) {
      params.query = searchRequest.query;
    }

    if (!!searchRequest.period) {
      params.from = searchRequest.period.from;
      params.to = searchRequest.period.to;
    }

    return params;
  }
}
