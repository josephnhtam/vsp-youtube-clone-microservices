import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChannelComponent } from './channel/channel.component';
import { ChannelSectionComponent } from './channel/channel-section/channel-section.component';
import { ChannelSectionRowComponent } from './channel/channel-section-row/channel-section-row.component';
import { ChannelResolver } from './channel/channel-resolver.service';
import { ChannelFeaturedComponent } from './channel/channel-featured/channel-featured.component';
import { ChannelVideosComponent } from './channel/channel-videos/channel-videos.component';
import { ChannelPlaylistsComponent } from './channel/channel-playlists/channel-playlists.component';
import { ChannelInfoComponent } from './channel/channel-info/channel-info.component';
import { SwiperModule } from 'swiper/angular';
import { ChannelFeaturedResolver } from './channel/channel-featured/channel-featured-resolver.service';
import { VideosSectionRowComponent } from './channel/channel-section-row/videos-section-row/videos-section-row.component';
import { CreatedPlaylistsSectionRowComponent } from './channel/channel-section-row/created-playlists-section-row/created-playlists-section-row.component';
import { SinglePlaylistSectionRowComponent } from './channel/channel-section-row/single-playlist-section-row/single-playlist-section-row.component';
import { MultiplePlaylistsSectionRowComponent } from './channel/channel-section-row/multiple-playlists-section-row/multiple-playlists-section-row.component';
import { ChannelPlaylistsOverviewComponent } from './channel/channel-playlists/channel-playlists-overview/channel-playlists-overview.component';
import { ChannelPlaylistsViewComponent } from './channel/channel-playlists/channel-playlists-view/channel-playlists-view.component';
import { ChannelPlaylistsViewSelectorComponent } from './channel/channel-playlists/channel-playlists-view-selector/channel-playlists-view-selector.component';
import { SpotlightVideoSectionRowComponent } from './channel/channel-section-row/spotlight-video-section-row/spotlight-video-section-row.component';
import { ChannelAboutComponent } from './channel/channel-about/channel-about.component';
import { ChannelPlaylistsResolver } from './channel/channel-playlists/channel-playlists-resolver.service';
import { CoreModule } from '../core/core.module';
import { SharedModule } from '../shared/shared.module';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatTabsModule } from '@angular/material/tabs';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ChannelRoutingModule } from './channel-routing.module';

// import Swiper core and required modules
import SwiperCore, { Navigation, Pagination } from 'swiper';

@NgModule({
  declarations: [
    ChannelComponent,
    ChannelSectionComponent,
    ChannelSectionRowComponent,
    ChannelFeaturedComponent,
    ChannelVideosComponent,
    ChannelPlaylistsComponent,
    ChannelInfoComponent,
    VideosSectionRowComponent,
    CreatedPlaylistsSectionRowComponent,
    SinglePlaylistSectionRowComponent,
    MultiplePlaylistsSectionRowComponent,
    ChannelPlaylistsOverviewComponent,
    ChannelPlaylistsViewComponent,
    ChannelPlaylistsViewSelectorComponent,
    SpotlightVideoSectionRowComponent,
    ChannelAboutComponent,
  ],
  providers: [
    ChannelResolver,
    ChannelFeaturedResolver,
    ChannelPlaylistsResolver,
  ],
  imports: [
    CommonModule,
    CoreModule,
    SharedModule,
    ChannelRoutingModule,
    SwiperModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatTabsModule,
    MatProgressSpinnerModule,
  ],
})
export class ChannelModule {
  constructor() {
    // install Swiper modules
    SwiperCore.use([Pagination, Navigation]);
  }
}
