import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { WatchVideoRoutingModule } from './watch-video-routing.module';
import { VideoPlaylistComponent } from './watch-video/video-playlist/video-playlist.component';
import { VideoPlaylistItemComponent } from './watch-video/video-playlist/video-playlist-item/video-playlist-item.component';
import { RelevantVideosComponent } from './watch-video/relevant-videos/relevant-videos.component';
import { WatchVideoComponent } from './watch-video/watch-video.component';
import { VideoActionComponent } from './watch-video/video-actions/video-actions.component';
import { VideoSubscriptionComponent } from './watch-video/video-subscription/video-subscription.component';
import { VideoActionService } from './watch-video/video-actions/video-actions.service';
import { CoreModule } from '../core/core.module';
import { SharedModule } from '../shared/shared.module';
import { CommunityModule } from '../community/community.module';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { VideoPlayerModule } from '../video-player/video-player.module';
import { VideoResolver } from './watch-video/video-resolver.service';

@NgModule({
  declarations: [
    VideoPlaylistComponent,
    VideoPlaylistItemComponent,
    RelevantVideosComponent,
    WatchVideoComponent,
    VideoActionComponent,
    VideoSubscriptionComponent,
  ],
  providers: [VideoResolver, VideoActionService],
  imports: [
    CommonModule,
    CoreModule,
    SharedModule,
    CommunityModule,
    VideoPlayerModule,
    WatchVideoRoutingModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
    MatMenuModule,
    MatProgressSpinnerModule,
  ],
})
export class WatchVideoModule {}
