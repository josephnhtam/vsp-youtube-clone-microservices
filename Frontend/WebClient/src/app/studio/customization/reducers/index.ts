import {BasicInfoUpdate, BrandingUpdate, LayoutUpdate, UserData,} from './../models/index';
import {createReducer, on} from '@ngrx/store';
import {CustomizationAction, CustomizationApiAction} from '../actions';

export interface State {
  userData: UserData | null;
  pending: boolean;
  error: any | null;
  layoutUpdate: LayoutUpdate | null;
  brandingUpdate: BrandingUpdate | null;
  basicInfoUpdate: BasicInfoUpdate | null;
  thumbnailToken: string | null;
  bannerToken: string | null;
}

const initialState: State = {
  userData: null,
  pending: false,
  error: null,
  layoutUpdate: null,
  brandingUpdate: null,
  basicInfoUpdate: null,
  thumbnailToken: null,
  bannerToken: null,
};

export const reducer = createReducer(
  initialState,

  on(CustomizationApiAction.bannerUploaded, (state, { bannerToken }) => {
    return {
      ...state,
      bannerToken,
    };
  }),

  on(CustomizationApiAction.thumbnailUploaded, (state, { thumbnailToken }) => {
    return {
      ...state,
      thumbnailToken,
    };
  }),

  on(CustomizationApiAction.failedToUpdateUser, (state, { error }) => {
    return {
      ...state,
      pending: false,
      error,
    };
  }),

  on(
    CustomizationAction.updateUser,
    (state, { layoutUpdate, brandingUpdate, basicInfoUpdate }) => {
      return {
        ...state,
        pending: true,
        error: null,
        layoutUpdate:
          layoutUpdate != null
            ? JSON.parse(JSON.stringify(layoutUpdate))
            : null,
        brandingUpdate: brandingUpdate != null ? { ...brandingUpdate } : null,
        basicInfoUpdate:
          basicInfoUpdate != null ? { ...basicInfoUpdate } : null,
        thumbnailToken: null,
        bannerToken: null,
      };
    }
  ),

  on(CustomizationApiAction.failedToRetrieveUserData, (state, { error }) => {
    return {
      ...state,
      pending: false,
      error,
    };
  }),

  on(CustomizationApiAction.userDataRetrieved, (state, { userData }) => {
    return {
      ...state,
      userData,
      pending: false,
      error: null,
    };
  }),

  on(CustomizationAction.retrieveUserData, (state) => {
    return {
      ...state,
      pending: true,
      error: null,
    };
  })
);
