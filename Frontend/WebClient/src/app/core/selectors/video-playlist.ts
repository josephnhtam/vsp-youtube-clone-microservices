import {isVideoAvailable} from 'src/app/core/models/library';
import {createSelector} from '@ngrx/store';
import {selectVideoPlaylistState} from '.';
import {adapter} from '../reducers/video-playlist';

const selectors = adapter.getSelectors();

export const selectVideoPlaylistItems = createSelector(
  selectVideoPlaylistState,
  (state) => selectors.selectAll(state)
);

export const selectAvailableVideoPlaylistItems = createSelector(
  selectVideoPlaylistItems,
  (items) => items.filter((item) => isVideoAvailable(item.video))
);

export const selectVideoPlaylistItem = (itemId: string) =>
  createSelector(selectVideoPlaylistState, (state) =>
    selectors.selectAll(state).find((x) => x.id == itemId)
  );
