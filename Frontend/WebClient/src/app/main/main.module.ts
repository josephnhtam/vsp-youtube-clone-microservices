import { CoreModule } from './../core/core.module';
import { SharedModule } from './../shared/shared.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MainRoutingModule } from './main-routing.module';
import { HomeComponent } from './home/home.component';
import { MainComponent } from './main.component';
import { SidebarComponent } from './sidebar/sidebar.component';
import { MiniSidebarComponent } from './mini-sidebar/mini-sidebar.component';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatBadgeModule } from '@angular/material/badge';
import { MatTooltipModule } from '@angular/material/tooltip';
import { NgScrollbarModule } from 'ngx-scrollbar';
import { SearchbarComponent } from './searchbar/searchbar.component';
import { ToolsComponent } from './tools/tools.component';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { SearchComponent } from './search/search.component';
import { NotificationsDialogComponent } from './notifications-dialog/notifications-dialog.component';
import { NotificationMessageComponent } from './notifications-dialog/notification-message/notification-message.component';
import { VideoUnavailableComponent } from './unavailable/video-unavailable/video-unavailable.component';
import { PlaylistUnavailableComponent } from './unavailable/playlist-unavailable/playlist-unavailable.component';
import { ChannelUnavailableComponent } from './unavailable/channel-unavailable/channel-unavailable.component';
import { SidebarService } from './sidebar/sidebar.service';
import { MatSidenavModule } from '@angular/material/sidenav';

@NgModule({
  declarations: [
    HomeComponent,
    MainComponent,
    SidebarComponent,
    MiniSidebarComponent,
    SearchbarComponent,
    ToolsComponent,
    SearchComponent,
    NotificationsDialogComponent,
    NotificationMessageComponent,
    VideoUnavailableComponent,
    PlaylistUnavailableComponent,
    ChannelUnavailableComponent,
  ],
  providers: [SidebarService],
  imports: [
    CommonModule,
    CoreModule,
    SharedModule,
    MainRoutingModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
    NgScrollbarModule,
    MatMenuModule,
    MatBadgeModule,
    MatProgressSpinnerModule,
    MatSidenavModule,
  ],
})
export class MainModule {}
