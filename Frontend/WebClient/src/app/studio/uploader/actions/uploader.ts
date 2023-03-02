import {ImageUploadRequest, VideoUploadRequest} from './../models/index';
import {createAction, props} from '@ngrx/store';

export const startUpload = createAction(
  '[uploader] start upload',
  props<{
    request: VideoUploadRequest | ImageUploadRequest;
  }>()
);

export const cancelUpload = createAction(
  '[uploader] cancel upload',
  props<{ uploadToken: string }>()
);
