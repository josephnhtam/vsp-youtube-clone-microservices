import {combineReducers} from '@ngrx/store';
import * as FromVideosManagement from './videos-management/reducers';
import * as FromUploader from './uploader/reducers';
import * as FromCustomization from './customization/reducers';

export const studioFeatureKey = 'studio';

export interface StudioState {
  videosManagement: FromVideosManagement.State;
  customization: FromCustomization.State;
  uploader: FromUploader.State;
}

export const studioReducer = combineReducers<StudioState>({
  videosManagement: FromVideosManagement.reducer,
  customization: FromCustomization.reducer,
  uploader: FromUploader.reducer,
});
