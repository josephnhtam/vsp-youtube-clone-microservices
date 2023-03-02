import {createAction, props} from '@ngrx/store';
import {VideoVisibility} from '../../models';

export const createVideo = createAction(
  '[videos management] create video',
  props<{
    title: string;
    description: string;
    videoFile?: File;
    contextId?: number;
  }>()
);

export const getVideoUploadToken = createAction(
  '[videos management] get video upload token',
  props<{ videoId: string; videoFile?: File; contextId?: number }>()
);

export const getVideo = createAction(
  '[videos management] get video',
  props<{ videoId: string }>()
);

export const getVideos = createAction(
  '[videos management] get videos',
  props<{ page: number; pageSize: number; sort: string | null }>()
);

export const clearLastFetchVideoClients = createAction(
  '[videos management] clear last fetch video clients'
);

export const setVideoInfo = createAction(
  '[videos management] set video info',
  props<{
    request: VideoInfoUpdateRequest;
  }>()
);

export const unregisterVideo = createAction(
  '[videos management] unregister video',
  props<{ videoId: string }>()
);

export interface VideoInfoUpdateRequest {
  videoId: string;

  setBasicInfo?: {
    title: string;
    description: string;
    tags: string;
    thumbnailId?: string;
  };

  setVisibilityInfo?: {
    visibility: VideoVisibility;
  };
}
