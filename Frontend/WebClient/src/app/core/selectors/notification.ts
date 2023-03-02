import {createSelector} from '@ngrx/store';
import {adapter} from '../reducers/notification';
import {selectNotificationState} from "./index";

const selectors = adapter.getSelectors();

export const selectHasMoreNotificationMessages = createSelector(
  selectNotificationState,
  (state) => {
    if (state.obtainedLastMessage) return false;
    return state.totalCount > state.ids.length;
  }
);

export const selectNotificationMessages = createSelector(
  selectNotificationState,
  (state) => selectors.selectAll(state)
);
