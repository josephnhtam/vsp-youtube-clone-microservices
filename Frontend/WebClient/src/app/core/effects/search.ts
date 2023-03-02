import {
  SearchByCreatorsRequest,
  SearchByQueryRequest,
  SearchByTagsRequest,
  SearchResponse,
} from '../models/search';
import { catchError, concatMap, map, Observable, of } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Actions, concatLatestFrom, createEffect, ofType } from '@ngrx/effects';
import { Injectable } from '@angular/core';
import { SearchAction, SearchApiAction } from '../actions';
import { Store } from '@ngrx/store';
import { environment } from 'src/environments/environment';
import { selectSearchState } from '../selectors';

@Injectable()
export class SearchEffects {
  constructor(
    private actions$: Actions,
    private httpClient: HttpClient,
    private store: Store
  ) {}

  loadMoreResultsEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(SearchAction.loadMoreResults),

      concatLatestFrom(() => this.store.select(selectSearchState)),

      concatMap(([_, state]) => {
        let search: Observable<SearchResponse>;

        const page = state.currentPage + 1;

        if (state.searchRequest!.type === 'query') {
          const url = environment.appSetup.apiUrl + '/api/v1/Search';

          const params = new HttpParams({
            fromObject: this.createHttpParams(
              state.searchRequest!,
              page,
              state.pageSize
            ),
          });

          search = this.httpClient.get<SearchResponse>(url, {
            params,
          });
        } else if (state.searchRequest!.type === 'tags') {
          const url = environment.appSetup.apiUrl + '/api/v1/Search/ByTags';

          search = this.httpClient.post<SearchResponse>(url, {
            tags: (state.searchRequest as SearchByTagsRequest).tags
              .split(',')
              .map((x) => x.trim()),
            page: page,
            pageSize: state.pageSize,
          });
        } else {
          const url = environment.appSetup.apiUrl + '/api/v1/Search/ByCreators';

          search = this.httpClient.post<SearchResponse>(url, {
            creatorIds: (state.searchRequest as SearchByCreatorsRequest)
              .creatorIds,
            page: page,
            pageSize: state.pageSize,
          });
        }

        return search.pipe(
          map((response) => {
            return SearchApiAction.searchResultObtained({
              contextId: state.contextId,
              items: response.items,
              totalCount: response.totalCount,
              page,
            });
          }),

          catchError((error) => {
            console.error(error);
            return of(
              SearchApiAction.failedToObtainSearchResult({
                contextId: state.contextId,
                error,
              })
            );
          })
        );
      })
    )
  );

  searchRelevantVidoes$ = createEffect(() =>
    this.actions$.pipe(
      ofType(SearchAction.searchRelevantVideos),

      concatLatestFrom(() => this.store.select(selectSearchState)),

      concatMap(([{ pageSize, video }, state]) => {
        const url =
          environment.appSetup.apiUrl + '/api/v1/Search/Tags/Relevant';

        const params = new HttpParams({
          fromObject: {
            tags: video.tags,
          },
        });

        return this.httpClient
          .get<string[]>(url, {
            params,
          })
          .pipe(
            map((tags) => {
              return SearchAction.search({
                searchRequest: {
                  type: 'tags',
                  tags: tags.join(','),
                },
                pageSize,
              });
            }),

            catchError((error) => {
              console.error(error);
              return of(
                SearchApiAction.failedToSearchRelevantVideos({
                  contextId: state.contextId,
                  error,
                })
              );
            })
          );
      })
    )
  );

  searchTrendingVideosEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(SearchAction.searchTrendingVideos),

      concatLatestFrom(() => this.store.select(selectSearchState)),

      concatMap(([{ pageSize }, state]) => {
        const url =
          environment.appSetup.apiUrl + '/api/v1/Search/Tags/Trending';

        return this.httpClient.get<string[]>(url).pipe(
          map((tags) => {
            return SearchAction.search({
              searchRequest: {
                type: 'tags',
                tags: tags.join(','),
              },
              pageSize,
            });
          }),

          catchError((error) => {
            console.error(error);
            return of(
              SearchApiAction.failedToSearchTrendingVideos({
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
      ofType(SearchAction.search),

      concatLatestFrom(() => this.store.select(selectSearchState)),

      concatMap(([{ searchRequest, pageSize }, state]) => {
        let search: Observable<SearchResponse>;

        if (searchRequest.type === 'query') {
          const url = environment.appSetup.apiUrl + '/api/v1/Search';

          const params = new HttpParams({
            fromObject: this.createHttpParams(searchRequest, 1, pageSize),
          });

          search = this.httpClient.get<SearchResponse>(url, {
            params,
          });
        } else if (searchRequest.type === 'tags') {
          const url = environment.appSetup.apiUrl + '/api/v1/Search/ByTags';

          search = this.httpClient.post<SearchResponse>(url, {
            tags: (state.searchRequest as SearchByTagsRequest).tags
              .split(',')
              .map((x) => x.trim()),
            page: 1,
            pageSize: pageSize,
          });
        } else {
          const url = environment.appSetup.apiUrl + '/api/v1/Search/ByCreators';

          search = this.httpClient.post<SearchResponse>(url, {
            creatorIds: (state.searchRequest as SearchByCreatorsRequest)
              .creatorIds,
            page: 1,
            pageSize: pageSize,
          });
        }

        return search.pipe(
          map((response) => {
            return SearchApiAction.searchResultObtained({
              contextId: state.contextId,
              items: response.items,
              totalCount: response.totalCount,
              page: 1,
            });
          }),

          catchError((error) => {
            console.error(error);
            return of(
              SearchApiAction.failedToObtainSearchResult({
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
    searchRequest:
      | SearchByQueryRequest
      | SearchByTagsRequest
      | SearchByCreatorsRequest,
    page: number,
    pageSize: number
  ) {
    if (searchRequest.type === 'query') {
      let params: any = {
        searchTarget: searchRequest.searchTarget,
        query: searchRequest.query,
        pageSize,
        page,
      };

      if (!!searchRequest.period) {
        params.from = searchRequest.period.from;
        params.to = searchRequest.period.to;
      }

      if (!!searchRequest.sort) {
        params.sort = searchRequest.sort;
      }

      return params;
    } else if (searchRequest.type === 'tags') {
      return {
        tags: searchRequest.tags,
        pageSize,
        page,
      };
    } else if (searchRequest.type === 'creators') {
      return {
        creatorIds: searchRequest.creatorIds.join(','),
        pageSize,
        page,
      };
    }
  }
}
