import {createAction, props} from '@ngrx/store';
import {UserProfile} from '../models/users';

export const userProfilesObtained = createAction(
  '[users / api] user profile obtained',
  props<{ userProfile: UserProfile }>()
);

export const failedToObtainUserProfiles = createAction(
  '[users / api] failed to obtain user profile',
  props<{ userId: string; error: any }>()
);

export const userCreated = createAction(
  '[users / api] user created',
  props<{ userId: string }>()
);

export const failedToCreateUser = createAction(
  '[users / api] failed to create user',
  props<{ userId: string; error: any }>()
);
