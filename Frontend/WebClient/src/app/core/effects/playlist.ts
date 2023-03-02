import { Store } from '@ngrx/store';
import {
  MovePlaylistItemByIdRequest,
  MovePlaylistItemRequest,
  Playlist,
} from '../models/library';
import { catchError, concatMap, map, mergeMap, of } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Actions, concatLatestFrom, createEffect, ofType } from '@ngrx/effects';
import { Injectable } from '@angular/core';
import { PlaylistAction, PlaylistApiAction } from '../actions';
import { environment } from 'src/environments/environment';
import { selectPlaylistState } from '../selectors';

@Injectable()
export class PlaylistEffects {
  constructor(
    private actions$: Actions,
    private store: Store,
    private httpClient: HttpClient
  ) {}

  removePlaylistVideoEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(PlaylistAction.removePlaylistVideo),

      concatLatestFrom(() => this.store.select(selectPlaylistState)),

      map(([{ itemId }, state]) => {
        const url =
          environment.appSetup.apiUrl + '/api/v1/PlaylistLibrary/Items/Video';

        const playlistId = state.id!;
        const videoId = state.entities[itemId]!.video.id;

        const params = new HttpParams({
          fromObject: {
            playlist: playlistId,
            videoId,
          },
        });

        this.httpClient
          .delete(url, {
            params,
          })
          .subscribe();

        return PlaylistApiAction.playlistItemRemoved({ playlistId, itemId });
      })
    )
  );

  removePlaylistItemEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(PlaylistAction.removePlaylistItem),

      concatLatestFrom(() => this.store.select(selectPlaylistState)),

      map(([{ itemId }, state]) => {
        const url =
          environment.appSetup.apiUrl + '/api/v1/PlaylistLibrary/Items';

        const playlistId = state.id!;

        const params = new HttpParams({
          fromObject: {
            playlist: playlistId,
            itemId,
          },
        });

        this.httpClient
          .delete(url, {
            params,
          })
          .subscribe();

        return PlaylistApiAction.playlistItemRemoved({ playlistId, itemId });
      })
    )
  );

  movePlaylistItemToBottomEffect$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(PlaylistAction.movePlaylistItemToBottom),

        concatLatestFrom(() => this.store.select(selectPlaylistState)),

        concatMap(([{ itemId }, state]) => {
          const url =
            environment.appSetup.apiUrl +
            '/api/v1/PlaylistLibrary/Items/MoveTo';

          const request: MovePlaylistItemRequest = {
            playlist: state.id!,
            itemId,
            toPosition: -1,
          };

          return this.httpClient.put<void>(url, request);
        })
      ),
    {
      dispatch: false,
    }
  );

  movePlaylistItemToTopEffect$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(PlaylistAction.movePlaylistItemToTop),

        concatLatestFrom(() => this.store.select(selectPlaylistState)),

        concatMap(([{ itemId }, state]) => {
          const url =
            environment.appSetup.apiUrl +
            '/api/v1/PlaylistLibrary/Items/MoveTo';

          const request: MovePlaylistItemRequest = {
            playlist: state.id!,
            itemId,
            toPosition: 0,
          };

          return this.httpClient.put<void>(url, request);
        })
      ),
    {
      dispatch: false,
    }
  );

  movePlaylistItemEffect$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(PlaylistAction.movePlaylistItem),

        concatLatestFrom(() => this.store.select(selectPlaylistState)),

        concatMap(([{ itemId, precedingItemId }, state]) => {
          const url =
            environment.appSetup.apiUrl + '/api/v1/PlaylistLibrary/Items/Move';

          const request: MovePlaylistItemByIdRequest = {
            playlist: state.id!,
            itemId,
            precedingItemId,
          };

          return this.httpClient.put<void>(url, request);
        })
      ),
    {
      dispatch: false,
    }
  );

  loadMoreVideosEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(PlaylistAction.loadMoreVideos),

      concatLatestFrom(() => this.store.select(selectPlaylistState)),

      mergeMap(([_, state]) => {
        const url = environment.appSetup.apiUrl + '/api/v1/PlaylistLibrary';

        const playlistId = state.id!;
        const page = state.currentPage + 1;
        const pageSize = state.pageSize;

        const params = new HttpParams({
          fromObject: {
            playlist: playlistId!,
            page,
            pageSize,
          },
        });

        return this.httpClient
          .get<Playlist>(url, {
            params,
          })
          .pipe(
            map((playlist) => {
              return PlaylistApiAction.videosLoaded({ playlist, page });
            }),

            catchError((error) => {
              console.error(error);
              return of(
                PlaylistApiAction.failedToLoadVideos({ playlistId, error })
              );
            })
          );
      })
    )
  );

  loadPlaylistEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(PlaylistAction.loadPlaylist),

      mergeMap(({ playlistId, pageSize }) => {
        const url = environment.appSetup.apiUrl + '/api/v1/PlaylistLibrary';

        const params = new HttpParams({
          fromObject: {
            playlist: playlistId,
            page: 1,
            pageSize,
          },
        });

        return this.httpClient
          .get<Playlist>(url, {
            params,
          })
          .pipe(
            map((playlist) => {
              return PlaylistApiAction.playlistLoaded({ playlist, pageSize });
            }),

            catchError((error) => {
              console.error(error);
              return of(
                PlaylistApiAction.failedToLoadPlaylist({ playlistId, error })
              );
            })
          );
      })
    )
  );
}
