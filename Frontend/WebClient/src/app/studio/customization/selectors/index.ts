import {selectCustomizationState} from './../../selectors';
import {createSelector} from '@ngrx/store';

export const selectUserData = createSelector(
  selectCustomizationState,
  (state) => state.userData
);

export const selectCustomizationPending = createSelector(
  selectCustomizationState,
  (state) => state.pending
);

export const selectCustomizationError = createSelector(
  selectCustomizationState,
  (state) => state.error
);
