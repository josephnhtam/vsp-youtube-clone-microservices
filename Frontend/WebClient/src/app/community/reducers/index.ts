import {combineReducers} from '@ngrx/store';
import * as FromVideoForum from './../video-forum/reducers';

export const communityFeatureKey = 'community';

export interface CommunityState {
  videoForum: FromVideoForum.State;
}

export const communityReducer = combineReducers<CommunityState>({
  videoForum: FromVideoForum.reducer,
});
