import { catchError, concatMap, map, of, switchMap } from 'rxjs';
import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import {
  PlaylistManagementAction,
  PlaylistManagementApiAction,
} from '../actions';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import {
  AddPlaylistToLibraryRequest,
  CreatePlaylistRequest,
  GetPlaylistRefResponse,
  GetSimplePlaylistInfosResponse,
  SimplePlaylistInfo,
  UpdatePlaylistRequest,
} from '../models/playlist';

@Injectable()
export class PlaylistManagementEffect {
  constructor(private actions$: Actions, private httpClient: HttpClient) {}

  checkPlaylistRefEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(PlaylistManagementAction.checkPlaylistRef),

      concatMap(({ playlistId, title }) => {
        const url = environment.appSetup.apiUrl + '/api/v1/PlaylistLibrary/Ref';

        const params = new HttpParams({
          fromObject: {
            playlistId,
          },
        });

        return this.httpClient
          .get<GetPlaylistRefResponse>(url, { params })
          .pipe(
            map((response) => {
              let simplePlaylistInfo: SimplePlaylistInfo | null = null;

              if (response.exists && title) {
                simplePlaylistInfo = {
                  id: playlistId,
                  title,
                  updateDate: response.createDate!,
                  parsedUpdateDate: new Date(response.createDate!),
                };
              }

              return PlaylistManagementApiAction.playlistRefChecked({
                playlistId,
                exists: response.exists,
                simplePlaylistInfo,
              });
            }),

            catchError((error) => {
              this.showErrorMsg(error);

              return of(
                PlaylistManagementApiAction.failedToCheckPlaylistRef({
                  playlistId,
                  error,
                })
              );
            })
          );
      })
    )
  );

  removePlaylistRefEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(PlaylistManagementAction.removePlaylistRef),

      concatMap(({ playlistId }) => {
        const url = environment.appSetup.apiUrl + '/api/v1/PlaylistLibrary/Ref';

        const params = new HttpParams({
          fromObject: {
            playlistId,
          },
        });

        return this.httpClient.delete(url, { params }).pipe(
          map(() => {
            return PlaylistManagementApiAction.playlistRefRemoved({
              playlistId,
            });
          }),

          catchError((error) => {
            this.showErrorMsg(error);

            return of(
              PlaylistManagementApiAction.failedToRemovePlaylistRef({
                playlistId,
                error,
              })
            );
          })
        );
      })
    )
  );

  createPlaylistRefEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(PlaylistManagementAction.createPlaylistRef),

      concatMap(({ playlistId, title }) => {
        const url = environment.appSetup.apiUrl + '/api/v1/PlaylistLibrary/Ref';

        const request: AddPlaylistToLibraryRequest = {
          playlistId,
        };

        return this.httpClient.post(url, request).pipe(
          map((id) => {
            let simplePlaylistInfo: SimplePlaylistInfo | null = null;

            if (title) {
              simplePlaylistInfo = {
                id: playlistId,
                title,
                updateDate: '',
                parsedUpdateDate: new Date(),
              };
            }

            return PlaylistManagementApiAction.playlistRefCreated({
              playlistId,
              simplePlaylistInfo,
            });
          }),

          catchError((error) => {
            this.showErrorMsg(error);

            return of(
              PlaylistManagementApiAction.failedToCreatePlaylistRef({
                playlistId,
                error,
              })
            );
          })
        );
      })
    )
  );

  removePlaylistEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(PlaylistManagementAction.removePlaylist),

      concatMap(({ playlistId }) => {
        const url = environment.appSetup.apiUrl + '/api/v1/PlaylistLibrary';

        const params = new HttpParams({
          fromObject: {
            playlistId,
          },
        });

        return this.httpClient
          .delete(url, {
            params,
          })
          .pipe(
            map(() => {
              return PlaylistManagementApiAction.playlistRemoved({
                playlistId,
              });
            }),

            catchError((error) => {
              console.error(error);
              return of(
                PlaylistManagementApiAction.failedToRemovePlaylist({
                  playlistId,
                  error,
                })
              );
            })
          );
      })
    )
  );

  updatePlaylistEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(PlaylistManagementAction.updatePlaylist),

      concatMap(({ playlistId, title, description, visibility }) => {
        const url = environment.appSetup.apiUrl + '/api/v1/PlaylistLibrary';

        const request: UpdatePlaylistRequest = {
          playlistId: playlistId,
          title: title ?? null,
          description: description ?? null,
          visibility: visibility ?? null,
        };

        return this.httpClient.put(url, request).pipe(
          map(() => {
            return PlaylistManagementApiAction.playlistUpdated({
              playlistId,
              title,
              description,
              visibility,
            });
          }),

          catchError((error) => {
            this.showErrorMsg(error);
            return of(
              PlaylistManagementApiAction.failedToUpdatePlaylist({
                playlistId,
                error,
              })
            );
          })
        );
      })
    )
  );

  retrieveSimplePlaylistInfosEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(PlaylistManagementAction.retrieveSimplePlaylistInfos),

      switchMap(({ maxCount }) => {
        const url =
          environment.appSetup.apiUrl +
          '/api/v1/PlaylistLibrary/SimpleInfos/Self';

        let params = new HttpParams();
        if (!!maxCount) {
          params = params.append('page', 1);
          params = params.append('pageSize', maxCount);
        }

        return this.httpClient
          .get<GetSimplePlaylistInfosResponse>(url, { params })
          .pipe(
            map((response) => {
              return PlaylistManagementApiAction.simplePlaylistInfosRetrieved({
                simplePlaylistInfos: response.infos,
                totalCount: response.totalCount,
              });
            }),

            catchError((error) => {
              this.showErrorMsg(error);

              return of(
                PlaylistManagementApiAction.failedToRetrieveSimplePlaylistInfos()
              );
            })
          );
      })
    )
  );

  createPlaylistEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(PlaylistManagementAction.createPlaylist),

      concatMap(({ title, description, visibility, contextId }) => {
        const url = environment.appSetup.apiUrl + '/api/v1/PlaylistLibrary';

        const request: CreatePlaylistRequest = {
          title: title,
          description: description,
          visibility: visibility,
        };

        return this.httpClient.post<string>(url, request).pipe(
          map((id) => {
            const simplePlaylistInfo: SimplePlaylistInfo = {
              id,
              title,
              updateDate: '',
              parsedUpdateDate: new Date(),
            };

            return PlaylistManagementApiAction.playlistCreated({
              playlistId: id,
              simplePlaylistInfo,
              contextId,
            });
          }),

          catchError((error) => {
            this.showErrorMsg(error);

            return of(
              PlaylistManagementApiAction.failedToCreatePlaylist({
                contextId,
                error,
              })
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
