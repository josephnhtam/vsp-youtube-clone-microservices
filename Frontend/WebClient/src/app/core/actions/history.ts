import {SearchUserWatchHistoryRequest} from '../models/history';
import {createAction, props} from '@ngrx/store';

export const search = createAction(
  '[history] search',
  props<{
    searchRequest: SearchUserWatchHistoryRequest;
    pageSize: number;
  }>()
);

export const loadMoreRecords = createAction('[history] load more records');

export const reset = createAction('[history] reset');

export const clearHistory = createAction('[history] clear history');

export const removeVideo = createAction(
  '[history] remove video',
  props<{ videoId: string }>()
);
