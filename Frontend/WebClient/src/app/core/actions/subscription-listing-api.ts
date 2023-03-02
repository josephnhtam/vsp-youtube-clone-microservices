import {createAction, props} from '@ngrx/store';
import {SubscriptionClient, SubscriptionTargetSort,} from '../models/subscription';

export const failedToObtainMoreSubscriptions = createAction(
  '[subscription listing / api] failed to obtain more subscriptions',
  props<{ contextId: number; error: any }>()
);

export const failedToObtainSubscriptions = createAction(
  '[subscription listing / api] failed to obtain subscriptions',
  props<{ contextId: number; error: any }>()
);

export const moreSubscriptionsObtained = createAction(
  '[subscription listing / api] more subscriptions obtained',
  props<{
    contextId: number;
    page: number;
    subscriptions: SubscriptionClient[];
  }>()
);

export const subscriptionsObtained = createAction(
  '[subscription listing / api] subscriptions obtained',
  props<{
    contextId: number;
    pageSize: number;
    sort: SubscriptionTargetSort;
    subscriptionsCount: number;
    subscriptions: SubscriptionClient[];
  }>()
);
