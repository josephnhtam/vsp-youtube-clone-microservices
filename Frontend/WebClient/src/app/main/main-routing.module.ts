import { ChannelUnavailableComponent } from './unavailable/channel-unavailable/channel-unavailable.component';
import { VideoUnavailableComponent } from './unavailable/video-unavailable/video-unavailable.component';
import { SearchComponent } from './search/search.component';
import { HomeComponent } from './home/home.component';
import { MainComponent } from './main.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes, UrlSegment } from '@angular/router';
import { PlaylistUnavailableComponent } from './unavailable/playlist-unavailable/playlist-unavailable.component';

const channelHandleMatcher = (url: UrlSegment[]) => {
  if (url.length > 0 && url[0].path.startsWith('@')) {
    return {
      consumed: [url[0]],
      posParams: {
        handle: new UrlSegment(url[0].path.slice(1), {}),
      },
    };
  }

  return null;
};

const routes: Routes = [
  {
    path: '',
    component: MainComponent,
    children: [
      {
        path: '',
        pathMatch: 'full',
        component: HomeComponent,
      },
      {
        path: 'search',
        component: SearchComponent,
      },
      {
        path: 'unavailable/video',
        component: VideoUnavailableComponent,
      },
      {
        path: 'unavailable/playlist',
        component: PlaylistUnavailableComponent,
      },
      {
        path: 'unavailable/channel',
        component: ChannelUnavailableComponent,
      },
      {
        path: 'watch',
        loadChildren: () =>
          import('../watch-video/watch-video.module').then(
            (m) => m.WatchVideoModule
          ),
      },
      {
        path: 'playlist',
        loadChildren: () =>
          import('../playlist/playlist.module').then((m) => m.PlaylistModule),
      },
      {
        path: 'library',
        loadChildren: () =>
          import('../library/library.module').then((m) => m.LibraryModule),
      },
      {
        path: 'history',
        loadChildren: () =>
          import('../history/history.module').then((m) => m.HistoryModule),
      },
      {
        path: 'subscriptions',
        loadChildren: () =>
          import('../subscriptions/subscriptions.module').then(
            (m) => m.SubscriptionsModule
          ),
      },
      {
        path: 'channels',
        loadChildren: () =>
          import('../channels/channels.module').then((m) => m.ChannelsModule),
      },
      {
        path: 'channel/:userId',
        loadChildren: () =>
          import('../channel/channel.module').then((m) => m.ChannelModule),
      },
      {
        matcher: channelHandleMatcher,
        loadChildren: () =>
          import('../channel/channel.module').then((m) => m.ChannelModule),
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class MainRoutingModule {}
