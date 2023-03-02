import {selectUserProfilesState} from './index';
import {createSelector} from '@ngrx/store';
import {adapter} from '../reducers/user-profiles';

const selectors = adapter.getSelectors();

export const selectAllUserProfileClients = createSelector(
  selectUserProfilesState,
  selectors.selectAll
);

export const selectUserProfileClient = (userId: string) =>
  createSelector(selectAllUserProfileClients, (userProfiles) =>
    userProfiles.find((x) => x.id === userId)
  );
