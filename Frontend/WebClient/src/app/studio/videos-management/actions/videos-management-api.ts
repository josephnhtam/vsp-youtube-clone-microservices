import {createAction, props} from '@ngrx/store';
import {GetVideosResponseDto} from '../../dto-models';
import {Video} from '../../models';

export const videoCreated = createAction(
  '[videos management / api] video created',
  props<{ video: Video; videoFile?: File; contextId?: number }>()
);

export const failedToCreateVideo = createAction(
  '[videos management / api] failed to create video',
  props<{ error: any; contextId?: number }>()
);

export const videoUploadTokenObtained = createAction(
  '[videos management / api] video upload token obtained',
  props<{
    videoId: string;
    videoUploadToken: string;
    videoFile?: File;
  }>()
);

export const failedToObtainVideoUploadToken = createAction(
  '[videos management / api] failed to obtain video upload token',
  props<{ error: any; videoId: string }>()
);

export const failedToUploadVideo = createAction(
  '[videos management / api] failed to upload video',
  props<{ videoId: string; error: any }>()
);

export const videoObtained = createAction(
  '[videos management / api] video obtained',
  props<{ video: Video }>()
);

export const videosObtained = createAction(
  '[videos management / api] videos obtained',
  props<{ response: GetVideosResponseDto }>()
);

export const failedToObtainVideos = createAction(
  '[videos management / api] failed to obtain videos'
);

export const videoInfoUpdated = createAction(
  '[videos management / api] video info updated',
  props<{ videoId: string; video: Video }>()
);

export const failedToUpdateVideoInfo = createAction(
  '[videos management / api] failed to update video info',
  props<{ videoId: string }>()
);

export const videoUnregistered = createAction(
  '[videos management / api] video unregistered',
  props<{ videoId: string }>()
);

export const failedToUnregisterVideo = createAction(
  '[videos management / api] failed to unregister video',
  props<{ videoId: string; error: any }>()
);
