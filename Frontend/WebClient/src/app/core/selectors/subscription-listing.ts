import {createSelector} from '@ngrx/store';
import {adapter} from '../reducers/subscription-listing';
import {selectSubscriptionListingState} from "./index";

const selectors = adapter.getSelectors();

export const selectHasMoreSubscriptions = createSelector(
  selectSubscriptionListingState,
  (state) => {
    if (state.obtainedLastSubscription) return false;
    return state.subscriptionsCount > state.ids.length;
  }
);

export const selectLoadingMoreSubscriptions = createSelector(
  selectSubscriptionListingState,
  (state) => state.loadingMoreSubscriptions
);

export const selectSubscriptions = createSelector(
  selectSubscriptionListingState,
  (state) => selectors.selectAll(state)
);

export const selectSubscription = (targetId: string) =>
  createSelector(
    selectSubscriptionListingState,
    (state) => state.entities[targetId]
  );
