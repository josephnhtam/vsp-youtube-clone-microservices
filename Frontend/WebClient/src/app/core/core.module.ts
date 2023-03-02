import { ChannelSectionsEffect } from './effects/channel-section';
import { PlaylistManagementEffect } from './effects/playlist-management';
import { CommonModule } from '@angular/common';
import { UserProfilesEffect } from './effects/users';
import { EffectsModule } from '@ngrx/effects';
import { coreFeatureKey, coreReducer } from './reducers/index';
import { NgModule } from '@angular/core';
import { StoreModule } from '@ngrx/store';
import { NFormatterPipe } from './pipes/n-formatter.pipe';
import { SubscriptionManagementEffects } from './effects/subscription-management';
import { PlaylistEffects } from './effects/playlist';
import { SearchEffects } from './effects/search';
import { HistoryEffects } from './effects/history';
import { SubscriptionListingEffects } from './effects/subscription-listing';
import { VideoPlaylistEffects } from './effects/video-playlist';
import { ChannelPageEffects } from './effects/channel-page';
import { NotificationEffect } from './effects/notification';
import { ChannelHelperService } from './services/channel-helper.service';
import { UserProfileService } from './services/user-profile.service';
import { ChannelSubscriptionService } from './services/channel-subscription.service';

@NgModule({
  declarations: [NFormatterPipe],
  providers: [
    UserProfileService,
    ChannelHelperService,
    ChannelSubscriptionService,
  ],
  imports: [
    CommonModule,
    StoreModule.forFeature(coreFeatureKey, coreReducer),
    EffectsModule.forFeature([
      UserProfilesEffect,
      PlaylistManagementEffect,
      SubscriptionManagementEffects,
      ChannelSectionsEffect,
      PlaylistEffects,
      SearchEffects,
      HistoryEffects,
      SubscriptionListingEffects,
      VideoPlaylistEffects,
      ChannelPageEffects,
      NotificationEffect,
    ]),
  ],
  exports: [NFormatterPipe],
})
export class CoreModule {}
