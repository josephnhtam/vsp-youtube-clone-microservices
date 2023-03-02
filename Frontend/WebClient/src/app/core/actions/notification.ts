import {createAction, props} from '@ngrx/store';

export const loadMessages = createAction(
  '[notification] load messages',
  props<{ pageSize: number }>()
);

export const loadMoreMessages = createAction(
  '[notification] load more messages'
);

export const reset = createAction('[notification] reset');

export const removeMessage = createAction(
  '[notification] remove message',
  props<{ messageId: string }>()
);

export const markMessageAsChecked = createAction(
  '[notification] mark message as checked',
  props<{ messageId: string }>()
);
