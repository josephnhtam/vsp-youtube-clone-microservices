import {createAction, props} from '@ngrx/store';
import {NotificationType, Subscription, UserProfile} from "../models/subscription";

export const notificationTypeChanged = createAction(
  '[subscription management / api] notification type changed',
  props<{ userId: string; notificationType: NotificationType }>()
);

export const failedToChangeNotificationType = createAction(
  '[subscription management / api] failed to change notification type',
  props<{ userId: string }>()
);

export const subscribed = createAction(
  '[subscription management / api] subscribed',
  props<{
    userId: string;
    notificationType: NotificationType;
    userProfile?: UserProfile;
  }>()
);

export const failedToSubscribe = createAction(
  '[subscription management / api] failed to subscribe',
  props<{ userId: string }>()
);

export const unsubscribed = createAction(
  '[subscription management / api] unsubscribed',
  props<{ userId: string }>()
);

export const failedToUnsubscribe = createAction(
  '[subscription management / api] failed to unsubscribe',
  props<{ userId: string }>()
);

export const subscriptionsRetrieved = createAction(
  '[subscription management / api] subscriptions retrieved',
  props<{ subscriptions: Subscription[] }>()
);

export const failedToRetrieveSubscriptions = createAction(
  '[subscription management / api] failed to retrieve subscriptions'
);
