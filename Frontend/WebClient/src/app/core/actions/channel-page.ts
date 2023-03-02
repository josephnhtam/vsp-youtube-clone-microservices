import {ChannelPageRequest} from '../reducers/channel-page';
import {createAction, props} from '@ngrx/store';

export const loadChannelPage = createAction(
  '[channel page] load channel page',
  props<{
    userId: string;
    contextId: string;
    request: ChannelPageRequest;
    pageSize: number;
  }>()
);

export const loadMoreResults = createAction(
  '[channel page] load more results',
  props<{
    userId: string;
    contextId: string;
  }>()
);

export const resetAll = createAction('[channel page] reset all');

export const reset = createAction(
  '[channel page] reset',
  props<{
    userId: string;
    contextId: string;
  }>()
);
