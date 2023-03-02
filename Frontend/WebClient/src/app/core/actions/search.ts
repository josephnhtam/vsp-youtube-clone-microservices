import {SearchByCreatorsRequest, SearchByQueryRequest, SearchByTagsRequest,} from '../models/search';
import {createAction, props} from '@ngrx/store';
import {Video} from '../models/video';

export const search = createAction(
  '[search] search',
  props<{
    searchRequest:
      | SearchByQueryRequest
      | SearchByTagsRequest
      | SearchByCreatorsRequest;
    pageSize: number;
  }>()
);

export const loadMoreResults = createAction('[search] load more results');

export const reset = createAction('[search] reset');

export const searchTrendingVideos = createAction(
  '[search] search trending videos',
  props<{ pageSize: number }>()
);

export const searchRelevantVideos = createAction(
  '[search] search relevant videos',
  props<{ pageSize: number; video: Video }>()
);
