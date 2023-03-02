import {createReducer, on} from '@ngrx/store';
import {NotificationMessage} from '../models/notification';
import {createEntityAdapter, EntityState} from '@ngrx/entity';
import {NotificationAction, NotificationApiAction} from '../actions';

export const adapter = createEntityAdapter<NotificationMessage>({
  selectId: (x) => x.id,
});

export interface State extends EntityState<NotificationMessage> {
  contextId: number;
  totalCount: number;
  currentPage: number;
  pageSize: number;
  loaded: boolean;
  pending: boolean;
  error: any | null;
  obtainedLastMessage: boolean;
}

const initialState: State = adapter.getInitialState({
  contextId: 0,
  totalCount: 0,
  currentPage: 1,
  pageSize: 0,
  loaded: false,
  pending: false,
  error: null,
  obtainedLastMessage: false,
});

export const reducer = createReducer<State>(
  initialState,

  on(
    NotificationApiAction.failedToMarkMessageAsChecked,
    NotificationApiAction.failedToRemoveMessage,
    (state, { error }) => {
      return {
        ...state,
        error,
      };
    }
  ),

  on(
    NotificationApiAction.failedToObtainMessages,
    (state, { contextId, error }) => {
      if (contextId != state.contextId) return state;

      return {
        ...state,
        pending: false,
        error,
      };
    }
  ),

  on(
    NotificationAction.markMessageAsChecked,
    NotificationApiAction.messageMarkedAsChecked,
    (state, { messageId }) => {
      return adapter.updateOne(
        {
          id: messageId,
          changes: {
            checked: true,
          },
        },
        {
          ...state,
        }
      );
    }
  ),

  on(
    NotificationAction.removeMessage,
    NotificationApiAction.messageRemoved,
    (state, { messageId }) => {
      return adapter.removeOne(messageId, {
        ...state,
      });
    }
  ),

  on(
    NotificationApiAction.messagesObtained,
    (state, { contextId, messages, totalCount, page }) => {
      if (contextId != state.contextId) return state;

      return adapter.upsertMany(messages, {
        ...state,
        totalCount,
        currentPage: page,
        loaded: true,
        pending: false,
        error: null,
        obtainedLastMessage: messages.length == 0,
      });
    }
  ),

  on(NotificationAction.reset, (state) => {
    return adapter.removeAll({
      ...state,
      contextId: state.contextId + 1,
      totalCount: 0,
      currentPage: 1,
      loaded: false,
      pending: false,
      error: null,
      obtainedLastMessage: false,
    });
  }),

  on(NotificationAction.loadMoreMessages, (state) => {
    return {
      ...state,
      pending: true,
      error: null,
    };
  }),

  on(NotificationAction.loadMessages, (state, { pageSize }) => {
    return adapter.removeAll({
      ...state,
      contextId: state.contextId + 1,
      totalCount: 0,
      currentPage: 1,
      pageSize,
      loaded: false,
      pending: true,
      error: null,
      obtainedLastMessage: false,
    });
  })
);
