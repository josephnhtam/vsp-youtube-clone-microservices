import {createEntityAdapter, EntityState} from '@ngrx/entity';
import {createReducer, on} from '@ngrx/store';
import {UsersAction, UsersApiAction} from '../actions';
import {UserProfileClient} from '../models/users';

export interface State extends EntityState<UserProfileClient> {}

export const adapter = createEntityAdapter<UserProfileClient>({
  selectId: (x) => x.id,
});

const initialState: State = adapter.getInitialState();

export const reducer = createReducer<State>(
  initialState,

  on(UsersAction.resetUserProfile, (state, { userId }) => {
    return adapter.removeOne(userId, { ...state });
  }),

  on(UsersApiAction.failedToObtainUserProfiles, (state, { userId, error }) => {
    return adapter.updateOne(
      {
        id: userId,
        changes: {
          pending: false,
          error,
        },
      },
      state
    );
  }),

  on(UsersApiAction.userProfilesObtained, (state, { userProfile }) => {
    return adapter.updateOne(
      {
        id: userProfile.id,
        changes: {
          userProfile,
          pending: false,
          error: null,
        },
      },
      state
    );
  }),

  on(UsersAction.getUserProfile, (state, { userId }) => {
    if (!state.entities[userId]) {
      return adapter.addOne(
        {
          id: userId,
          pending: true,
          userProfile: null,
          error: null,
        },
        state
      );
    } else {
      return adapter.updateOne(
        {
          id: userId,
          changes: {
            pending: true,
            error: null,
          },
        },
        state
      );
    }
  })
);
