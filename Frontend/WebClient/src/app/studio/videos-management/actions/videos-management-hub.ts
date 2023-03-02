import {Video, VideoThumbnail} from '../../models/index';
import {createAction, props} from '@ngrx/store';
import {ProcessedVideo} from '../../models';

export const videoUploaded = createAction(
  '[videos-management / hub] video uploaded',
  props<{
    videoId: string;
    originalFileName: string;
    videoFileUrl: string;
  }>()
);

export const videoBeingProcessed = createAction(
  '[videos-management / hub] video being processed',
  props<{
    videoId: string;
  }>()
);

export const videoProcessingFailed = createAction(
  '[videos-management / hub] video processing failed',
  props<{
    videoId: string;
  }>()
);

export const videoProcessingComplete = createAction(
  '[videos-management / hub] video processing complete',
  props<{
    video: Video;
  }>()
);

export const processedVideoAdded = createAction(
  '[videos-management / hub] processed video added',
  props<{
    videoId: string;
    video: ProcessedVideo;
  }>()
);

export const videoThumbnailsAdded = createAction(
  '[videos-management / hub] video thumbnails added',
  props<{
    videoId: string;
    thumbnails: VideoThumbnail[];
  }>()
);
