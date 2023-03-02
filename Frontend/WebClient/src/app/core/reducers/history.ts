import {createEntityAdapter, EntityState} from '@ngrx/entity';
import {createReducer, on} from '@ngrx/store';
import {HistoryAction, HistoryApiAction} from '../actions';
import {SearchUserWatchHistoryRequest, UserWatchRecord,} from '../models/history';

export const adapter = createEntityAdapter<UserWatchRecord>({
  selectId: (v) => v.id,
});

export interface State extends EntityState<UserWatchRecord> {
  contextId: number;
  searchRequest: SearchUserWatchHistoryRequest | null;
  currentPage: number;
  pageSize: number;
  loaded: boolean;
  pending: boolean;
  loadingMoreRecords: boolean;
  error: any | null;
  totalCount: number;
  obtainedLastRecord: boolean;
}

const initialState: State = adapter.getInitialState({
  contextId: 0,
  searchRequest: null,
  currentPage: 1,
  pageSize: 50,
  loaded: false,
  pending: false,
  loadingMoreRecords: false,
  totalCount: 0,
  error: null,
  obtainedLastRecord: false,
});

export const reducer = createReducer<State>(
  initialState,

  on(HistoryApiAction.videoRemoved, (state, { contextId, videoId }) => {
    if (state.contextId !== contextId) return state;

    const videosToRemove = adapter
      .getSelectors()
      .selectAll(state)
      .filter((x) => x.video.id === videoId);

    return adapter.removeMany(
      videosToRemove.map((x) => x.id),
      {
        ...state,
        pending: false,
        loadingMoreRecords: false,
        totalCount: state.totalCount - videosToRemove.length,
      }
    );
  }),

  on(HistoryAction.removeVideo, (state) => {
    return {
      ...state,
      pending: true,
      error: null,
    };
  }),

  on(
    HistoryApiAction.failedToObtainHistoryRecord,
    HistoryApiAction.failedToClearHistory,
    HistoryApiAction.failedToRemoveVideo,
    (state, { contextId, error }) => {
      if (state.contextId !== contextId) return state;

      return {
        ...state,
        pending: false,
        loadingMoreRecords: false,
        error,
      };
    }
  ),

  on(HistoryAction.loadMoreRecords, (state, {}) => {
    return {
      ...state,
      pending: true,
      loadingMoreRecords: true,
      error: null,
    };
  }),

  on(
    HistoryApiAction.historyRecordsObtained,
    (state, { contextId, items, totalCount }) => {
      if (contextId !== state.contextId) return state;

      items = items.map((item) => {
        return {
          ...item,
          parsedDate: new Date(item.date),
        };
      });

      return adapter.upsertMany(items, {
        ...state,
        loaded: true,
        pending: false,
        loadingMoreRecords: false,
        error: null,
        totalCount,
        obtainedLastRecord: items.length === 0,
      });
    }
  ),

  on(HistoryAction.search, (state, { searchRequest }) => {
    return adapter.removeAll({
      ...state,
      contextId: state.contextId + 1,
      searchRequest,
      loaded: false,
      pending: true,
      loadingMoreRecords: false,
      currentPage: 1,
      error: null,
      totalCount: 0,
    });
  }),

  on(HistoryAction.reset, (state) => {
    return adapter.removeAll({
      ...state,
      contextId: state.contextId + 1,
      loaded: false,
      pending: false,
      loadingMoreRecords: false,
      error: null,
      totalCount: 0,
    });
  }),

  on(HistoryAction.clearHistory, (state) => {
    return {
      ...state,
      contextId: state.contextId + 1,
      pending: true,
      loadingMoreRecords: false,
      error: null,
    };
  }),

  on(HistoryApiAction.historyCleared, (state) => {
    if (!state.loaded) {
      return adapter.removeAll({
        ...state,
        contextId: state.contextId + 1,
        pending: false,
        totalCount: 0,
      });
    } else {
      return adapter.removeAll({
        ...state,
        contextId: state.contextId + 1,
        currentPage: 1,
        pending: false,
        loadingMoreRecords: false,
        error: null,
        totalCount: 0,
      });
    }
  })
);
