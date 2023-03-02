import {createSelector} from '@ngrx/store';
import {adapter} from '../reducers/history';
import {selectHistoryState} from "./index";

const selectors = adapter.getSelectors();

export const selectHasMoreHistoryRecords = createSelector(
  selectHistoryState,
  (state) => {
    if (state.obtainedLastRecord) return false;
    return state.totalCount > state.ids.length;
  }
);

export const selectHistoryRecords = createSelector(
  selectHistoryState,
  (state) => selectors.selectAll(state)
);
