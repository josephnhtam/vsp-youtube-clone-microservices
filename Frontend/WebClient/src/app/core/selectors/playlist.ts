import {isVideoAvailable} from 'src/app/core/models/library';
import {createSelector} from '@ngrx/store';
import {adapter} from '../reducers/playlist';
import {selectPlaylistState} from "./index";

const selectors = adapter.getSelectors();

export const selectHasMoreVideos = createSelector(
  selectPlaylistState,
  (state) => {
    if (state.obtainedLastVideo) return false;
    return state.videosCount > state.ids.length;
  }
);

export const selectLoadingMoreVideos = createSelector(
  selectPlaylistState,
  (state) => state.loadingMoreVideos
);

export const selectPlaylistItems = createSelector(
  selectPlaylistState,
  (state) => selectors.selectAll(state)
);

export const selectAvailablePlaylistItems = createSelector(
  selectPlaylistItems,
  (items) => items.filter((item) => isVideoAvailable(item.video))
);

export const selectPlaylistItem = (itemId: string) =>
  createSelector(selectPlaylistState, (state) =>
    selectors.selectAll(state).find((x) => x.id == itemId)
  );
