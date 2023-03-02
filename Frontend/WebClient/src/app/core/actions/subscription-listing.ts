import {createAction, props} from '@ngrx/store';
import {NotificationType, SubscriptionTargetSort} from "../models/subscription";

export const getSubscriptions = createAction(
  '[subscription listing] get subscriptions',
  props<{
    pageSize: number;
    sort: SubscriptionTargetSort;
  }>()
);

export const getMoreSubscriptions = createAction(
  '[subscription listing] get more subscriptions'
);

export const updateSubscriptionData = createAction(
  '[subscription listing] update subscription data',
  props<{
    targetId: string;
    notificationType: NotificationType;
    isSubscribed: boolean;
  }>()
);
