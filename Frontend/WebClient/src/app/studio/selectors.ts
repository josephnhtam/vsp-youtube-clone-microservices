import {createFeatureSelector, createSelector} from '@ngrx/store';
import {studioFeatureKey, StudioState} from './reducers';

export const selectFeatureState =
  createFeatureSelector<StudioState>(studioFeatureKey);

export const selectVideosManagementState = createSelector(
  selectFeatureState,
  (state) => state.videosManagement
);

export const selectCustomizationState = createSelector(
  selectFeatureState,
  (state) => state.customization
);

export const selectUploaderState = createSelector(
  selectFeatureState,
  (state) => state.uploader
);
