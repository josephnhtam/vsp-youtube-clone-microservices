import {createEntityAdapter, EntityState} from '@ngrx/entity';
import {createReducer, on} from '@ngrx/store';
import {SubscriptionManagementAction, SubscriptionManagementApiAction,} from '../actions';
import {Subscription} from "../models/subscription";

export interface State extends EntityState<Subscription> {
  retrievingInfos: boolean;
}

export const adapter = createEntityAdapter<Subscription>({
  selectId: (x) => x.userProfile.id,
  sortComparer: (a, b) => {
    return (
      b.parsedSubscriptionDate.getTime() - a.parsedSubscriptionDate.getTime()
    );
  },
});

const initialState: State = adapter.getInitialState({
  retrievingInfos: false,
});

export const reducer = createReducer<State>(
  initialState,

  on(SubscriptionManagementApiAction.unsubscribed, (state, { userId }) => {
    return adapter.removeOne(userId, {
      ...state,
    });
  }),

  on(
    SubscriptionManagementApiAction.subscribed,
    (state, { userId, userProfile }) => {
      if (!userProfile) {
        return { ...state };
      }

      const subscriptionInfo: Subscription = {
        userProfile,
        subscriptionDate: '',
        parsedSubscriptionDate: new Date(),
      };

      return adapter.upsertOne(subscriptionInfo, {
        ...state,
      });
    }
  ),

  on(SubscriptionManagementApiAction.failedToRetrieveSubscriptions, (state) => {
    return {
      ...state,
      retrievingInfos: false,
    };
  }),

  on(
    SubscriptionManagementApiAction.subscriptionsRetrieved,
    (state, { subscriptions }) => {
      subscriptions = subscriptions.map((info) => {
        return {
          ...info,
          parsedSubscriptionDate: new Date(info.subscriptionDate),
        };
      });

      return adapter.upsertMany(subscriptions, {
        ...state,
        retrievingInfos: false,
      });
    }
  ),

  on(SubscriptionManagementAction.retrieveSubscriptions, (state) => {
    return adapter.removeAll({
      ...state,
      retrievingInfos: true,
    });
  }),

  on(SubscriptionManagementAction.clearSubscriptions, (state) => {
    return adapter.removeAll({
      ...state,
      retrievingInfos: false,
    });
  })
);
