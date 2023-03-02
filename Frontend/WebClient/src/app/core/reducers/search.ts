import {createReducer, on} from '@ngrx/store';
import {createEntityAdapter, EntityState} from '@ngrx/entity';
import {SearchableItem, SearchByCreatorsRequest, SearchByQueryRequest, SearchByTagsRequest,} from '../models/search';
import {SearchAction, SearchApiAction} from '../actions';

export const adapter = createEntityAdapter<SearchableItem>({
  selectId: (v) => v.id,
});

export interface State extends EntityState<SearchableItem> {
  contextId: number;
  searchRequest:
    | SearchByQueryRequest
    | SearchByTagsRequest
    | SearchByCreatorsRequest
    | null;
  currentPage: number;
  pageSize: number;
  loaded: boolean;
  pending: boolean;
  loadingMoreResults: boolean;
  error: any | null;
  totalCount: number;
  obtainedLastResult: boolean;
  tags: string[];
  creatorIds: string[];
}

const initialState: State = adapter.getInitialState({
  contextId: 0,
  searchRequest: null,
  currentPage: 1,
  pageSize: 50,
  loaded: false,
  pending: false,
  loadingMoreResults: false,
  totalCount: 0,
  error: null,
  obtainedLastResult: false,
  tags: [],
  creatorIds: [],
});

export const reducer = createReducer<State>(
  initialState,

  on(
    SearchApiAction.failedToSearchTrendingVideos,
    SearchApiAction.failedToSearchRelevantVideos,
    (state, { contextId, error }) => {
      if (state.contextId !== contextId) return state;

      return {
        ...state,
        pending: false,
        loadingMoreResults: false,
        error,
      };
    }
  ),

  on(
    SearchAction.searchTrendingVideos,
    SearchAction.searchRelevantVideos,
    (state) => {
      return adapter.removeAll({
        ...state,
        contextId: state.contextId + 1,
        searchRequest: null,
        loaded: false,
        pending: true,
        loadingMoreResults: false,
        currentPage: 1,
        error: null,
        tags: [],
      });
    }
  ),

  on(
    SearchApiAction.failedToObtainSearchResult,
    (state, { contextId, error }) => {
      if (state.contextId !== contextId) return state;

      return {
        ...state,
        pending: false,
        loadingMoreResults: false,
        error,
      };
    }
  ),

  on(SearchAction.loadMoreResults, (state, {}) => {
    return {
      ...state,
      pending: true,
      loadingMoreResults: true,
      error: null,
    };
  }),

  on(
    SearchApiAction.searchResultObtained,
    (state, { contextId, items, totalCount, page }) => {
      if (contextId !== state.contextId) return state;

      return adapter.upsertMany(items, {
        ...state,
        loaded: true,
        pending: false,
        loadingMoreResults: false,
        error: null,
        totalCount,
        obtainedLastResult: items.length === 0,
        currentPage: page,
      });
    }
  ),

  on(SearchAction.search, (state, { searchRequest, pageSize }) => {
    return adapter.removeAll({
      ...state,
      contextId: state.contextId + 1,
      searchRequest,
      loaded: false,
      pending: true,
      loadingMoreResults: false,
      currentPage: 1,
      pageSize,
      error: null,
      tags:
        searchRequest.type === 'tags'
          ? searchRequest.tags.split(',').map((x) => x.trim())
          : [],
      creatorIds:
        searchRequest.type === 'creators' ? searchRequest.creatorIds : [],
      totalCount: 0,
    });
  }),

  on(SearchAction.reset, (state) => {
    return adapter.removeAll({
      ...state,
      contextId: state.contextId + 1,
      loaded: false,
      pending: false,
      loadingMoreResults: false,
      error: null,
      tags: [],
      totalCount: 0,
    });
  })
);
