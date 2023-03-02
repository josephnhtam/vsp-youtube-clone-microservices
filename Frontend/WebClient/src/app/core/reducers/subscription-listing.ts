import {createEntityAdapter, EntityState} from '@ngrx/entity';
import {createReducer, on} from '@ngrx/store';
import {SubscriptionListingAction, SubscriptionListingApiAction, SubscriptionManagementApiAction,} from '../actions';
import {SubscriptionClient, SubscriptionTargetSort,} from '../models/subscription';

export interface State extends EntityState<SubscriptionClient> {
  contextId: number;
  loaded: boolean;
  pending: boolean;
  loadingMoreSubscriptions: boolean;
  error: any | null;
  currentPage: number;
  pageSize: number;
  sort: SubscriptionTargetSort;
  subscriptionsCount: number;
  obtainedLastSubscription: boolean;
}

export const adapter = createEntityAdapter<SubscriptionClient>({
  selectId: (x) => x.userProfile.id,
});

const initialState = adapter.getInitialState({
  contextId: 0,
  loaded: false,
  pending: false,
  loadingMoreSubscriptions: false,
  error: null,
  currentPage: 1,
  pageSize: 50,
  sort: SubscriptionTargetSort.DisplayName,
  subscriptionsCount: 0,
  obtainedLastSubscription: false,
});

export const reducer = createReducer<State>(
  initialState,

  on(
    SubscriptionManagementApiAction.notificationTypeChanged,
    (state, { userId, notificationType }) => {
      return adapter.updateOne(
        {
          id: userId,
          changes: {
            notificationType,
            isSubscribed: true,
          },
        },
        {
          ...state,
        }
      );
    }
  ),

  on(
    SubscriptionManagementApiAction.subscribed,
    (state, { userId, notificationType, userProfile }) => {
      return adapter.updateOne(
        {
          id: userId,
          changes: {
            notificationType,
            isSubscribed: true,
          },
        },
        {
          ...state,
        }
      );
    }
  ),

  on(SubscriptionManagementApiAction.unsubscribed, (state, { userId }) => {
    return adapter.updateOne(
      {
        id: userId,
        changes: {
          isSubscribed: false,
        },
      },
      {
        ...state,
      }
    );
  }),

  on(
    SubscriptionListingAction.updateSubscriptionData,
    (state, { targetId, notificationType, isSubscribed }) => {
      const subscription = state.entities[targetId] as SubscriptionClient;
      const userProfile = subscription.userProfile;

      const subscribersCountChange =
        subscription?.isSubscribed != isSubscribed
          ? isSubscribed
            ? 1
            : -1
          : 0;

      return adapter.updateOne(
        {
          id: targetId,
          changes: {
            notificationType,
            isSubscribed,
            userProfile: {
              ...userProfile,
              subscribersCount:
                userProfile.subscribersCount + subscribersCountChange,
            },
          },
        },
        {
          ...state,
        }
      );
    }
  ),

  on(
    SubscriptionListingApiAction.failedToObtainMoreSubscriptions,
    (state, { contextId, error }) => {
      if (contextId != state.contextId) return state;

      return {
        ...state,
        loadingMoreSubscriptions: false,
        error,
      };
    }
  ),

  on(
    SubscriptionListingApiAction.moreSubscriptionsObtained,
    (state, { contextId, page, subscriptions }) => {
      if (contextId != state.contextId) return state;

      const obtainedLastSubscription = subscriptions.length == 0;

      return adapter.upsertMany(subscriptions, {
        ...state,
        currentPage: obtainedLastSubscription
          ? state.currentPage
          : Math.max(state.currentPage, page),
        obtainedLastSubscription: obtainedLastSubscription,
        loadingMoreSubscriptions: false,
      });
    }
  ),

  on(SubscriptionListingAction.getMoreSubscriptions, (state) => {
    return {
      ...state,
      loadingMoreSubscriptions: true,
    };
  }),

  on(
    SubscriptionListingApiAction.failedToObtainSubscriptions,
    (state, { contextId, error }) => {
      if (contextId != state.contextId) {
        return state;
      }

      return {
        ...state,
        pending: false,
        error,
      };
    }
  ),

  on(
    SubscriptionListingApiAction.subscriptionsObtained,
    (
      state,
      { contextId, pageSize, sort, subscriptionsCount, subscriptions }
    ) => {
      if (contextId != state.contextId) return state;

      return adapter.upsertMany(subscriptions, {
        ...state,
        pageSize,
        sort,
        subscriptionsCount,
        loaded: true,
        pending: false,
        error: null,
      });
    }
  ),

  on(
    SubscriptionListingAction.getSubscriptions,
    (state, { pageSize, sort }) => {
      return adapter.removeAll({
        ...state,
        contextId: state.contextId + 1,
        pageSize,
        sort,
        loaded: false,
        pending: true,
        error: null,
      });
    }
  )
);
