import {selectSubscriptionManagementState} from './index';
import {adapter} from '../reducers/subscription-management';
import {createSelector} from '@ngrx/store';

const selectors = adapter.getSelectors();

export const selectSubscriptionInfos = createSelector(
  selectSubscriptionManagementState,
  selectors.selectAll
);

export const selectIsRetrievingSubscriptionInfos = createSelector(
  selectSubscriptionManagementState,
  (state) => state.retrievingInfos
);
