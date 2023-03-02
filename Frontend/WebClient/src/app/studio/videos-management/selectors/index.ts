import {createSelector} from '@ngrx/store';
import {selectVideosManagementState} from '../../selectors';

export const selectVideoClients = createSelector(
  selectVideosManagementState,
  (state) => state.videoClients
);

export const selectVideoClientById = (videoId: string) =>
  createSelector(selectVideoClients, (state) =>
    state.find((vc) => vc.video?.id === videoId)
  );

export const selectVideoClientByContextId = (contextId: number) =>
  createSelector(selectVideoClients, (state) =>
    state.find((vc) => vc.contextId === contextId)
  );

export const selectTotalVideosCount = createSelector(
  selectVideosManagementState,
  (state) => state.totalVideosCount
);

export const selectIsFetchingVideos = createSelector(
  selectVideosManagementState,
  (state) => state.isFetchingVideos
);

export const selectLastFetchVideoClients = createSelector(
  selectVideosManagementState,
  (state) => {
    return state.lastFetchVideoClientIds
      .map((id: string) => state.videoClients.find((vc) => vc.video?.id == id)!)
      .filter((x) => !!x);
  }
);
