import { VideoUploadRequest } from './../../uploader/models/index';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, concatMap, filter, map, mergeMap, of } from 'rxjs';
import { VideosManagementAction, VideosManagementApiAction } from '../actions';
import {
  CreateVideoRequestDto,
  GetVideosResponseDto,
  VideoUploadTokenResponseDto,
} from '../../dto-models';
import { Injectable } from '@angular/core';
import { startUpload } from '../../uploader/actions/uploader';
import { Video } from '../../models';

@Injectable()
export class VideosManagementEffect {
  constructor(private actions$: Actions, private httpClient: HttpClient) {}

  unregisterVideoEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(VideosManagementAction.unregisterVideo),
      concatMap(({ videoId }) => {
        const url =
          environment.appSetup.apiUrl +
          `/api/v1/VideoManager/Videos/${videoId}`;

        return this.httpClient.delete<Video>(url).pipe(
          map((video) => {
            return VideosManagementApiAction.videoUnregistered({
              videoId,
            });
          }),

          catchError((error) => {
            console.error(error);
            return of(
              VideosManagementApiAction.failedToUnregisterVideo({
                videoId,
                error,
              })
            );
          })
        );
      })
    )
  );

  setVideoInfoEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(VideosManagementAction.setVideoInfo),
      concatMap(({ request }) => {
        const url = environment.appSetup.apiUrl + '/api/v1/VideoManager/Videos';

        return this.httpClient.put<Video>(url, request).pipe(
          map((video) => {
            return VideosManagementApiAction.videoInfoUpdated({
              videoId: request.videoId,
              video,
            });
          }),

          catchError((error) => {
            console.error(error);
            return of(
              VideosManagementApiAction.failedToUpdateVideoInfo({
                videoId: request.videoId,
              })
            );
          })
        );
      })
    )
  );

  getVideosEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(VideosManagementAction.getVideos),
      mergeMap(({ page, pageSize, sort }) => {
        const url = environment.appSetup.apiUrl + '/api/v1/VideoManager/Videos';

        const params = new HttpParams({
          fromObject: {
            page,
            pageSize,
            sort: sort ?? '',
          },
        });

        return this.httpClient
          .get<GetVideosResponseDto>(url, {
            params,
          })
          .pipe(
            map((response) => {
              return VideosManagementApiAction.videosObtained({
                response,
              });
            }),

            catchError((error) => {
              console.error(error);
              return of(VideosManagementApiAction.failedToObtainVideos());
            })
          );
      })
    )
  );

  getVideoEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(VideosManagementAction.getVideo),

      mergeMap(({ videoId }) => {
        const url =
          environment.appSetup.apiUrl +
          `/api/v1/VideoManager/Videos/${videoId}`;

        return this.httpClient.get<Video>(url).pipe(
          map((videoDto) => {
            return VideosManagementApiAction.videoObtained({
              video: videoDto,
            });
          })
        );
      })
    )
  );

  videoUploadTokenObtainedEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(VideosManagementApiAction.videoUploadTokenObtained),

      filter((x) => !!x.videoFile),

      map(({ videoId, videoUploadToken, videoFile }) => {
        const request: VideoUploadRequest = {
          type: 'video',
          videoId,
          uploadToken: videoUploadToken,
          file: videoFile!,
        };

        return startUpload({
          request,
        });
      })
    )
  );

  getVideoUploadTokenEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(VideosManagementAction.getVideoUploadToken),

      concatMap(({ videoId, videoFile, contextId }) => {
        const url =
          environment.appSetup.apiUrl +
          '/api/v1/VideoManager/Videos/' +
          videoId +
          '/UploadToken';

        return this.httpClient.get<VideoUploadTokenResponseDto>(url).pipe(
          map((response) => {
            console.log('Video upload token obtained', response);

            return VideosManagementApiAction.videoUploadTokenObtained({
              videoId,
              videoUploadToken: response.videoUploadToken,
              videoFile,
            });
          }),

          catchError((err) => {
            console.error('Failed to obtain video upload token', err);

            return of(
              VideosManagementApiAction.failedToObtainVideoUploadToken({
                error: err.toString(),
                videoId,
              })
            );
          })
        );
      })
    )
  );

  videoCreatedEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(VideosManagementApiAction.videoCreated),

      filter((x) => !!x.videoFile),

      map(({ video, videoFile, contextId }) => {
        return VideosManagementAction.getVideoUploadToken({
          videoId: video.id,
          videoFile,
          contextId,
        });
      })
    )
  );

  createVideoEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(VideosManagementAction.createVideo),

      concatMap(({ title, description, videoFile, contextId }) => {
        const url = environment.appSetup.apiUrl + '/api/v1/VideoManager/Videos';

        const request: CreateVideoRequestDto = {
          title,
          description,
        };

        return this.httpClient.post<Video>(url, request).pipe(
          map((videoDto) => {
            return videoDto;
          }),

          map((video) => {
            console.log('Video created', video);

            return VideosManagementApiAction.videoCreated({
              video,
              videoFile,
              contextId,
            });
          }),

          catchError((err) => {
            console.error('Failed to create video', err);

            return of(
              VideosManagementApiAction.failedToCreateVideo({
                error: err.toString(),
                contextId,
              })
            );
          })
        );
      })
    )
  );
}
