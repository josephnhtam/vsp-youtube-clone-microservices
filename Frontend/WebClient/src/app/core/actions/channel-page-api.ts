import {ChannelPageContent, ChannelPageRequest,} from '../reducers/channel-page';
import {createAction, props} from '@ngrx/store';

export const channelPageObtained = createAction(
  '[channel page] channel page obtained',
  props<{
    userId: string;
    contextId: string;
    request: ChannelPageRequest;
    currentPage: number;
    pageSize: number;
    content: ChannelPageContent;
  }>()
);

export const failedToObtainChannelPage = createAction(
  '[channel page] failed to obtain channel page',
  props<{
    userId: string;
    contextId: string;
    error: any;
  }>()
);
