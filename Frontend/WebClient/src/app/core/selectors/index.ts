import {coreFeatureKey, CoreState} from './../reducers/index';
import {createFeatureSelector, createSelector} from '@ngrx/store';

export const selectFeatureState =
  createFeatureSelector<CoreState>(coreFeatureKey);

export const selectUserProfilesState = createSelector(
  selectFeatureState,
  (state) => state.userProfiles
);

export const selectPlaylistManagementState = createSelector(
  selectFeatureState,
  (state) => state.playlistManagement
);

export const selectSubscriptionManagementState = createSelector(
  selectFeatureState,
  (state) => state.subscriptionManagement
);

export const selectChannelSectionsState = createSelector(
  selectFeatureState,
  (state) => state.channelSections
);
export const selectPlaylistState = createSelector(
  selectFeatureState,
  (state) => state.playlist
);
export const selectSearchState = createSelector(
  selectFeatureState,
  (state) => state.search
);
export const selectHistoryState = createSelector(
  selectFeatureState,
  (state) => state.history
);
export const selectSubscriptionListingState = createSelector(
  selectFeatureState,
  (state) => state.subscriptionListing
);
export const selectVideoPlaylistState = createSelector(
  selectFeatureState,
  (state) => state.videoPlaylist
);
export const selectChannelPageState = createSelector(
  selectFeatureState,
  (state) => state.channelPage
);
export const selectNotificationState = createSelector(
  selectFeatureState,
  (state) => state.notification
);
