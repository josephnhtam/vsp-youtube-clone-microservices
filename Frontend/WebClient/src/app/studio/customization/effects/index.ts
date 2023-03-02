import { UserData } from './../models/index';
import { selectCustomizationState } from './../../selectors';
import { Action, Store } from '@ngrx/store';
import { environment } from './../../../../environments/environment';
import { catchError, filter, map, mergeMap, of, switchMap, take } from 'rxjs';
import { Actions, concatLatestFrom, createEffect, ofType } from '@ngrx/effects';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CustomizationAction, CustomizationApiAction } from '../actions';
import { UpdateUserProfileRequest } from '../models';
import { UploaderAction, UploaderApiAction } from '../../uploader/actions';
import { ImageUploadRequest, ImageUploadResponse } from '../../uploader/models';
import { State } from '../reducers';

@Injectable()
export class CustomizationEffects {
  constructor(
    private store: Store,
    private actions$: Actions,
    private httpClient: HttpClient
  ) {}

  userUpdatedEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(CustomizationApiAction.userUpdated),
      map(() => {
        return CustomizationAction.retrieveUserData();
      })
    )
  );

  doUpdateUserEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(CustomizationAction.doUpdateUser),

      concatLatestFrom(() => this.store.select(selectCustomizationState)),

      switchMap(([_, state]) => {
        const updateBasicInfo = this.createUpdateBasicInfo(state);
        const updateImages = this.createUpdateImages(state);
        const updateLayout = this.createUpdateLayout(state);

        const request: UpdateUserProfileRequest = {
          updateBasicInfo,
          updateImages,
          updateLayout,
        };

        const url = environment.appSetup.apiUrl + '/api/v1/Users';

        return this.httpClient.put(url, request).pipe(
          map(() => {
            return CustomizationApiAction.userUpdated();
          }),

          catchError((error) => {
            console.error(error);
            return of(CustomizationApiAction.failedToUpdateUser({ error }));
          })
        );
      })
    )
  );

  bannerUploadedEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(CustomizationApiAction.bannerUploaded),
      map(() => {
        return CustomizationAction.doUpdateUser();
      })
    )
  );

  thumbnailUploadedEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(CustomizationApiAction.thumbnailUploaded),

      concatLatestFrom(() => this.store.select(selectCustomizationState)),

      map(([_, state]) => {
        if (
          state.brandingUpdate?.bannerChanged &&
          state.brandingUpdate.newBannerFile != null
        ) {
          return CustomizationAction.uploadBanner();
        }
        return CustomizationAction.doUpdateUser();
      })
    )
  );

  uploadBannerEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(CustomizationAction.uploadBanner),

      concatLatestFrom(() => this.store.select(selectCustomizationState)),

      switchMap(([_, state]) => {
        const url =
          environment.appSetup.apiUrl + '/api/v1/Users/Banner/UploadToken';

        return this.httpClient.get<{ uploadToken: string }>(url).pipe(
          switchMap(({ uploadToken }) => {
            const imageUploadRequest: ImageUploadRequest = {
              type: 'image',
              file: state.brandingUpdate!.newBannerFile!,
              uploadToken,
            };

            return this.handleImageUpload(
              imageUploadRequest,
              (success, response) => {
                if (success) {
                  return CustomizationApiAction.bannerUploaded({
                    bannerToken: response!.token,
                  });
                } else {
                  return CustomizationApiAction.failedToUpdateUser({
                    error: 'failed to upload banner',
                  });
                }
              }
            );
          }),

          catchError((error) => {
            console.error(error);
            return of(CustomizationApiAction.failedToUpdateUser({ error }));
          })
        );
      })
    )
  );

  uploadThumbnailEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(CustomizationAction.uploadThumbnail),

      concatLatestFrom(() => this.store.select(selectCustomizationState)),

      switchMap(([_, state]) => {
        const url =
          environment.appSetup.apiUrl + '/api/v1/Users/Thumbnail/UploadToken';

        return this.httpClient.get<{ uploadToken: string }>(url).pipe(
          switchMap(({ uploadToken }) => {
            const imageUploadRequest: ImageUploadRequest = {
              type: 'image',
              file: state.brandingUpdate!.newThubmnailFile!,
              uploadToken,
            };

            return this.handleImageUpload(
              imageUploadRequest,
              (success, response) => {
                if (success) {
                  return CustomizationApiAction.thumbnailUploaded({
                    thumbnailToken: response!.token,
                  });
                } else {
                  return CustomizationApiAction.failedToUpdateUser({
                    error: 'failed to upload thumbnail',
                  });
                }
              }
            );
          }),

          catchError((error) => {
            console.error(error);
            return of(CustomizationApiAction.failedToUpdateUser({ error }));
          })
        );
      })
    )
  );

  updateUserEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(CustomizationAction.updateUser),

      map(({ brandingUpdate }) => {
        if (
          brandingUpdate?.thumbnailChanged &&
          brandingUpdate.newThubmnailFile != null
        ) {
          return CustomizationAction.uploadThumbnail();
        } else if (
          brandingUpdate?.bannerChanged &&
          brandingUpdate?.newBannerFile != null
        ) {
          return CustomizationAction.uploadBanner();
        } else {
          return CustomizationAction.doUpdateUser();
        }
      })
    )
  );

  retrieveDetailedUserProfileEffect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(CustomizationAction.retrieveUserData),

      mergeMap(() => {
        const url = environment.appSetup.apiUrl + '/api/v1/Users';

        return this.httpClient.get<UserData>(url).pipe(
          map((userData) => {
            return CustomizationApiAction.userDataRetrieved({
              userData,
            });
          }),

          catchError((error) => {
            console.error(error);
            return of(
              CustomizationApiAction.failedToRetrieveUserData({
                error,
              })
            );
          })
        );
      })
    )
  );

  private createUpdateBasicInfo(state: State) {
    return state.basicInfoUpdate == null
      ? null
      : {
          displayName: state.basicInfoUpdate.displayName,
          description: state.basicInfoUpdate.description,
          email:
            state.basicInfoUpdate.email.trim() != ''
              ? state.basicInfoUpdate.email.trim()
              : null,
          handle:
            state.basicInfoUpdate.handle.trim() != ''
              ? state.basicInfoUpdate.handle.trim()
              : null,
        };
  }

  private createUpdateImages(state: State) {
    return state.brandingUpdate == null
      ? null
      : {
          thumbnailChanged: state.brandingUpdate.thumbnailChanged,
          bannerChanged: state.brandingUpdate.bannerChanged,
          newThubmnailToken: state.thumbnailToken,
          newBannerToken: state.bannerToken,
        };
  }

  private createUpdateLayout(state: State) {
    return state.layoutUpdate == null
      ? null
      : {
          ...state.layoutUpdate,
        };
  }

  handleImageUpload(
    request: ImageUploadRequest,
    callback: (success: boolean, response?: ImageUploadResponse) => Action
  ) {
    this.store.dispatch(UploaderAction.startUpload({ request }));

    return this.actions$.pipe(
      ofType(
        UploaderApiAction.uploadSuccessful,
        UploaderApiAction.uploadFailed
      ),

      filter(({ uploadToken }) => uploadToken === request.uploadToken),

      take(1),

      map((action) => {
        if (action.type === UploaderApiAction.uploadSuccessful.type) {
          return callback(true, action.response as ImageUploadResponse);
        } else {
          return callback(false);
        }
      })
    );
  }
}
