import {
  ImageUploadRequest,
  ImageUploadResponse,
  UploadProcess,
  VideoUploadRequest,
} from './../models/index';
import { Store } from '@ngrx/store';
import { catchError, concatMap, filter, map, of, takeUntil, tap } from 'rxjs';
import { HttpClient, HttpEventType } from '@angular/common/http';
import { Actions, concatLatestFrom, createEffect, ofType } from '@ngrx/effects';
import { Injectable } from '@angular/core';
import { UploaderAction, UploaderApiAction } from '../actions';
import { environment } from 'src/environments/environment';
import { UploadStatus } from '../models';
import { uploadProcessAlreadyExists } from '../actions/uploader-api';
import { selectUploadProcess } from '../selectors';
import {
  VideosManagementAction,
  VideosManagementApiAction as VmApiAction,
} from '../../videos-management/actions';
import { CustomizationApiAction } from '../../customization/actions';

@Injectable()
export class UploaderEffects {
  constructor(
    private actions$: Actions,
    private httpClient: HttpClient,
    private store: Store
  ) {}

  cancelUploadEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(UploaderAction.cancelUpload),

      concatLatestFrom(({ uploadToken }) =>
        this.store.select(selectUploadProcess(uploadToken))
      ),

      filter(([_, process]) => {
        return !process || process.status != UploadStatus.InProgress;
      }),

      map(([{ uploadToken }, process]) => {
        return UploaderApiAction.uploadCancellationFailed({ uploadToken });
      })
    )
  );

  imageUploadFailedEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(UploaderApiAction.uploadFailed),

      concatLatestFrom(({ uploadToken }) =>
        this.store.select(selectUploadProcess(uploadToken))
      ),

      filter(([_, uploadProcess]) => uploadProcess?.request.type === 'image'),

      map(([_, uploadProcess]) => {
        const request = uploadProcess?.request as ImageUploadRequest;

        return CustomizationApiAction.failedToUploadImage({
          request,
          error: 'Failed to upload image',
        });
      })
    )
  );

  imageStartUploadEffect$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(UploaderAction.startUpload),

        filter((x) => x.request.type === 'image'),

        concatLatestFrom(({ request }) =>
          this.store.select(selectUploadProcess(request.uploadToken))
        ),

        filter(([{ request }, existingProcess]) => {
          if (existingProcess) {
            this.store.dispatch(
              uploadProcessAlreadyExists({ uploadToken: request.uploadToken })
            );
            return false;
          }
          return true;
        }),

        tap(([{ request }]) => {
          const uploadProcess: UploadProcess = {
            request,
            status: UploadStatus.InProgress,
            progress: 0,
            createDate: new Date(),
          };

          this.store.dispatch(
            UploaderApiAction.uploadProcessCreated({
              uploadProcess,
            })
          );
        }),

        concatMap(([{ request }]) => {
          const url = environment.appSetup.storageUrl + '/api/v1/ImageStorage';

          const formData = new FormData();
          formData.append('file', request.file);

          const params = {
            token: request.uploadToken,
          };

          return this.upload(request, url, params, formData);
        })
      ),
    { dispatch: false }
  );

  videoUploadFailedEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(UploaderApiAction.uploadFailed),

      concatLatestFrom(({ uploadToken }) =>
        this.store.select(selectUploadProcess(uploadToken))
      ),

      filter(([_, uploadProcess]) => uploadProcess?.request.type === 'video'),

      map(([_, uploadProcess]) => {
        const request = uploadProcess?.request as VideoUploadRequest;

        return VmApiAction.failedToUploadVideo({
          videoId: request.videoId,
          error: 'Failed to upload video',
        });
      })
    )
  );

  videoStartUploadEffect$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(UploaderAction.startUpload),

        filter((x) => x.request.type === 'video'),

        concatLatestFrom(({ request }) =>
          this.store.select(selectUploadProcess(request.uploadToken))
        ),

        filter(([{ request }, existingProcess]) => {
          if (existingProcess) {
            this.store.dispatch(
              uploadProcessAlreadyExists({ uploadToken: request.uploadToken })
            );
            return false;
          }
          return true;
        }),

        tap(([{ request }]) => {
          const uploadProcess: UploadProcess = {
            request,
            status: UploadStatus.InProgress,
            progress: 0,
            createDate: new Date(),
          };

          this.store.dispatch(
            UploaderApiAction.uploadProcessCreated({
              uploadProcess,
            })
          );
        }),

        concatMap(([{ request }]) => {
          const url = environment.appSetup.storageUrl + '/api/v1/VideoStorage';

          const formData = new FormData();
          formData.append('file', request.file);

          const params = {
            token: request.uploadToken,
          };

          return this.upload(request, url, params, formData);
        })
      ),
    { dispatch: false }
  );

  upload(
    request: VideoUploadRequest | ImageUploadRequest,
    url: string,
    params: any,
    formData: FormData
  ) {
    const httpPost = this.httpClient.post(url, formData, {
      params,
      reportProgress: true,
      observe: 'events',
    });

    return httpPost.pipe(
      takeUntil(
        this.actions$.pipe(
          ofType(UploaderAction.cancelUpload),

          filter(({ uploadToken }) => {
            return request.uploadToken === uploadToken;
          }),

          tap(() => {
            this.store.dispatch(
              UploaderApiAction.uploadCancelled({
                uploadToken: request.uploadToken,
              })
            );

            if (request.type === 'video') {
              this.store.dispatch(
                VideosManagementAction.unregisterVideo({
                  videoId: request.videoId,
                })
              );
            }
          })
        )
      ),

      takeUntil(
        this.actions$.pipe(
          ofType(VideosManagementAction.unregisterVideo),

          filter(({ videoId }) => {
            return request.type === 'video' && request.videoId === videoId;
          }),

          tap(() => {
            this.store.dispatch(
              UploaderApiAction.uploadCancelled({
                uploadToken: request.uploadToken,
              })
            );
          })
        )
      ),

      tap((ev) => {
        if (ev.type === HttpEventType.UploadProgress) {
          const progress = Math.round(100 * (ev.loaded / ev.total!));

          this.store.dispatch(
            UploaderApiAction.uploadProgressUpdated({
              uploadToken: request.uploadToken,
              progress,
            })
          );
        } else if (ev.type === HttpEventType.Response) {
          if (request.type === 'image') {
            this.store.dispatch(
              UploaderApiAction.uploadSuccessful({
                uploadToken: request.uploadToken,
                response: ev.body as ImageUploadResponse,
              })
            );
          } else {
            this.store.dispatch(
              UploaderApiAction.uploadSuccessful({
                uploadToken: request.uploadToken,
                response: null,
              })
            );
          }
        }
      }),
      catchError((error) => {
        console.error(error);
        return of(
          UploaderApiAction.uploadFailed({
            uploadToken: request.uploadToken,
          })
        );
      })
    );
  }
}
