import {communityFeatureKey, CommunityState} from './../reducers/index';
import {createFeatureSelector, createSelector} from '@ngrx/store';

export const selectCommunityState =
  createFeatureSelector<CommunityState>(communityFeatureKey);

export const selectVideoForumState = createSelector(
  selectCommunityState,
  (state) => state.videoForum
);
