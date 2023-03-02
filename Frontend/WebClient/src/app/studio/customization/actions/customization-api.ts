import {ImageUploadRequest} from './../../uploader/models/index';
import {UserData} from './../models/index';
import {createAction, props} from '@ngrx/store';

export const failedToRetrieveUserData = createAction(
  '[customization / api] failed to retrieve user data',
  props<{ error: any }>()
);

export const userDataRetrieved = createAction(
  '[customization / api] user data retrieved',
  props<{ userData: UserData }>()
);

export const failedToUploadImage = createAction(
  '[customization / api] failed to upload image',
  props<{ request: ImageUploadRequest; error: any }>()
);

export const userUpdated = createAction('[customization / api] user updated');

export const failedToUpdateUser = createAction(
  '[customization / api] failed to update user',
  props<{ error: any }>()
);

export const thumbnailUploaded = createAction(
  '[customization / api] thumbnail uploaded',
  props<{ thumbnailToken: string }>()
);

export const bannerUploaded = createAction(
  '[customization / api] banner uploaded',
  props<{ bannerToken: string }>()
);
