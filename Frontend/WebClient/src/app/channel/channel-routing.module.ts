import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {ChannelAboutComponent} from './channel/channel-about/channel-about.component';
import {ChannelFeaturedResolver} from './channel/channel-featured/channel-featured-resolver.service';
import {ChannelFeaturedComponent} from './channel/channel-featured/channel-featured.component';
import {ChannelPlaylistsResolver} from './channel/channel-playlists/channel-playlists-resolver.service';
import {ChannelPlaylistsComponent} from './channel/channel-playlists/channel-playlists.component';
import {ChannelResolver} from './channel/channel-resolver.service';
import {ChannelVideosComponent} from './channel/channel-videos/channel-videos.component';
import {ChannelComponent} from './channel/channel.component';

const routes: Routes = [
  {
    path: '',
    resolve: { channelData: ChannelResolver },
    component: ChannelComponent,
    runGuardsAndResolvers: 'paramsChange',
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'featured',
      },
      {
        path: 'featured',
        resolve: {
          channel: ChannelFeaturedResolver,
        },
        component: ChannelFeaturedComponent,
      },
      {
        path: 'videos',
        component: ChannelVideosComponent,
      },
      {
        path: 'playlists',
        resolve: {
          playlistsChannel: ChannelPlaylistsResolver,
        },
        component: ChannelPlaylistsComponent,
      },
      {
        path: 'about',
        component: ChannelAboutComponent,
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ChannelRoutingModule {}
