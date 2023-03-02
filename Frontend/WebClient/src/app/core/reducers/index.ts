import {combineReducers} from '@ngrx/store';
import * as FromUserProfile from './user-profiles';
import * as FromPlaylistManagement from './playlist-management';
import * as FromSubscriptionMangagement from './subscription-management';
import * as FromChannelSections from './channel-sections';
import * as FromPlaylists from "./playlist";
import * as FromSearch from "./search";
import * as FromHistory from "./history";
import * as FromSubscriptionListing from "./subscription-listing";
import * as FromVideoPlaylists from "./video-playlist";
import * as FromChannelPage from "./channel-page";
import * as FromNotification from "./notification";

export const coreFeatureKey = 'core';

export interface CoreState {
  userProfiles: FromUserProfile.State;
  playlistManagement: FromPlaylistManagement.State;
  subscriptionManagement: FromSubscriptionMangagement.State;
  channelSections: FromChannelSections.State;
  playlist: FromPlaylists.State;
  search: FromSearch.State;
  history: FromHistory.State;
  subscriptionListing: FromSubscriptionListing.State;
  videoPlaylist: FromVideoPlaylists.State;
  channelPage: FromChannelPage.State;
  notification: FromNotification.State;
}

export const coreReducer = combineReducers<CoreState>({
  userProfiles: FromUserProfile.reducer,
  playlistManagement: FromPlaylistManagement.reducer,
  subscriptionManagement: FromSubscriptionMangagement.reducer,
  channelSections: FromChannelSections.reducer,
  playlist: FromPlaylists.reducer,
  search: FromSearch.reducer,
  history: FromHistory.reducer,
  subscriptionListing: FromSubscriptionListing.reducer,
  videoPlaylist: FromVideoPlaylists.reducer,
  channelPage: FromChannelPage.reducer,
  notification: FromNotification.reducer,
});
