import {SearchableItem} from '../models/search';
import {createAction, props} from '@ngrx/store';

export const searchResultObtained = createAction(
  '[search / api] search result obtained',
  props<{
    contextId: number;
    items: SearchableItem[];
    totalCount: number;
    page: number;
  }>()
);

export const failedToObtainSearchResult = createAction(
  '[search / api] failed to obtain search result',
  props<{ contextId: number; error: any }>()
);

export const failedToSearchTrendingVideos = createAction(
  '[search / api] failed to search trending videos',
  props<{ contextId: number; error: any }>()
);

export const failedToSearchRelevantVideos = createAction(
  '[search / api] failed to search relevant videos',
  props<{ contextId: number; error: any }>()
);
