import {createAction, props} from '@ngrx/store';

export const getUserProfile = createAction(
  '[users] get user profile',
  props<{ userId: string }>()
);

export const createUser = createAction(
  '[users] create user',
  props<{ displayName: string | null; thumbnailToken: string | null }>()
);

export const resetUserProfile = createAction(
  '[users] reset user profile',
  props<{ userId: string }>()
);
