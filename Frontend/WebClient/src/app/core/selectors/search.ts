import {createSelector} from '@ngrx/store';
import {selectSearchState} from '.';
import {adapter} from '../reducers/search';

const selectors = adapter.getSelectors();

export const selectHasMoreSearchResults = createSelector(
  selectSearchState,
  (state) => {
    if (state.obtainedLastResult) return false;
    return state.totalCount > state.ids.length;
  }
);

export const selectSearchResults = createSelector(selectSearchState, (state) =>
  selectors.selectAll(state)
);
