import { Playlist } from '../models/library';
import { catchError, concatMap, map, of } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { Injectable } from '@angular/core';
import { VideoPlaylistAction, VideoPlaylistApiAction } from '../actions';
import { environment } from 'src/environments/environment';

@Injectable()
export class VideoPlaylistEffects {
  constructor(private actions$: Actions, private httpClient: HttpClient) {}

  loadPlaylistEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(VideoPlaylistAction.loadPlaylist),

      concatMap(({ playlistId, index, videoId, pageSize }) => {
        const url =
          environment.appSetup.apiUrl + '/api/v1/PlaylistLibrary/Near';

        const paramsObject: any = {
          playlist: playlistId,
          videoId,
          pageSize,
        };

        if (index != null) {
          paramsObject.index = index;
        }

        const params = new HttpParams({
          fromObject: paramsObject,
        });

        return this.httpClient
          .get<Playlist>(url, {
            params,
          })
          .pipe(
            map((playlist) => {
              return VideoPlaylistApiAction.playlistLoaded({
                playlistId,
                playlist,
                index,
                videoId,
                pageSize,
              });
            }),

            catchError((error) => {
              console.error(error);
              return of(
                VideoPlaylistApiAction.failedToLoadPlaylist({
                  playlistId,
                  error,
                })
              );
            })
          );
      })
    )
  );
}
