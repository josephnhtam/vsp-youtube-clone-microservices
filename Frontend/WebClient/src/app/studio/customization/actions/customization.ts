import {BasicInfoUpdate, BrandingUpdate, LayoutUpdate,} from './../models/index';
import {createAction, props} from '@ngrx/store';

export const retrieveUserData = createAction(
  '[customization] retrieve user data'
);

export const updateUser = createAction(
  '[customization] update user',
  props<{
    layoutUpdate: LayoutUpdate | null;
    brandingUpdate: BrandingUpdate | null;
    basicInfoUpdate: BasicInfoUpdate | null;
  }>()
);

export const doUpdateUser = createAction('[customization] do update user');
export const uploadThumbnail = createAction('[customization] upload thumbnail');
export const uploadBanner = createAction('[customization] upload banner');
