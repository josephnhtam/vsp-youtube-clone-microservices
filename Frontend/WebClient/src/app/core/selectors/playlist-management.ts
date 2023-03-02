import {selectPlaylistManagementState} from './index';
import {createSelector} from '@ngrx/store';
import {adapter} from '../reducers/playlist-management';

const selectors = adapter.getSelectors();

export const selectSimplePlaylistInfos = createSelector(
  selectPlaylistManagementState,
  selectors.selectAll
);

export const selectIsRetrievingSimplePlaylistInfos = createSelector(
  selectPlaylistManagementState,
  (state) => state.retrievingInfos
);
