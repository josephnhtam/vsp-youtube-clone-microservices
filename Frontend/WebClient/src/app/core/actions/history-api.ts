import {createAction, props} from '@ngrx/store';
import {UserWatchRecord} from '../models/history';

export const historyRecordsObtained = createAction(
  '[history / api] history records obtained',
  props<{
    contextId: number;
    items: UserWatchRecord[];
    totalCount: number;
    page: number;
  }>()
);

export const failedToObtainHistoryRecord = createAction(
  '[history / api] failed to obtain history record',
  props<{ contextId: number; error: any }>()
);

export const videoRemoved = createAction(
  '[history / api] video removed',
  props<{ contextId: number; videoId: string }>()
);

export const failedToRemoveVideo = createAction(
  '[history / api] failed to remove video',
  props<{ contextId: number; error: any }>()
);

export const historyCleared = createAction(
  '[history / api] history cleared',
  props<{ contextId: number }>()
);

export const failedToClearHistory = createAction(
  '[history / api] failed to clear history',
  props<{ contextId: number; error: any }>()
);
