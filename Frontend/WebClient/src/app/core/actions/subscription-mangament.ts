import {createAction, props} from '@ngrx/store';
import {NotificationType, UserProfile} from "../models/subscription";

export const changeNotificationType = createAction(
  '[subscription management] create playlist',
  props<{
    userId: string;
    notificationType: NotificationType;
  }>()
);

export const subscribe = createAction(
  '[subscription management] subscribe',
  props<{
    userId: string;
    notificationType: NotificationType;
    userProfile?: UserProfile;
  }>()
);

export const unsubscribe = createAction(
  '[subscription management] unsubscribe',
  props<{
    userId: string;
  }>()
);

export const retrieveSubscriptions = createAction(
  '[subscription management] retrieve subscription',
  props<{ maxCount?: number }>()
);

export const clearSubscriptions = createAction(
  '[subscription management] clear subscription'
);
