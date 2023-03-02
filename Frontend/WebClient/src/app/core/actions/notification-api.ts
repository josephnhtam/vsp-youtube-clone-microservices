import {NotificationMessage} from '../models/notification';
import {createAction, props} from '@ngrx/store';

export const messagesObtained = createAction(
  '[notification / api] message obtained',
  props<{
    contextId: number;
    messages: NotificationMessage[];
    totalCount: number;
    page: number;
  }>()
);

export const failedToObtainMessages = createAction(
  '[notification / api] failed to obtain message',
  props<{ contextId: number; error: any }>()
);

export const messageRemoved = createAction(
  '[notification / api] message removed',
  props<{ messageId: string }>()
);

export const failedToRemoveMessage = createAction(
  '[notification / api] failed to remove message',
  props<{ error: any }>()
);

export const messageMarkedAsChecked = createAction(
  '[notification / api] message marked as checked',
  props<{ messageId: string }>()
);

export const failedToMarkMessageAsChecked = createAction(
  '[notification / api] failed to mark message as checked',
  props<{ messageId: string; error: any }>()
);
