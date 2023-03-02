import {ImageUploadResponse, UploadProcess} from '../models/index';
import {createAction, props} from '@ngrx/store';

export const uploadProcessCreated = createAction(
  '[uploader / api] upload processs created',
  props<{ uploadProcess: UploadProcess }>()
);

export const uploadSuccessful = createAction(
  '[uploader / api] upload successful',
  props<{ uploadToken: string; response: ImageUploadResponse | null }>()
);

export const uploadFailed = createAction(
  '[uploader / api] upload failed',
  props<{ uploadToken: string }>()
);

export const uploadProgressUpdated = createAction(
  '[uploader / api] upload progress updated',
  props<{ uploadToken: string; progress: number }>()
);

export const uploadProcessAlreadyExists = createAction(
  '[uploader / api] upload process already exists',
  props<{ uploadToken: string }>()
);

export const uploadCancelled = createAction(
  '[uploader / api] upload cancelled',
  props<{ uploadToken: string }>()
);

export const uploadCancellationFailed = createAction(
  '[uploader / api] upload cancellation failed',
  props<{ uploadToken: string }>()
);
