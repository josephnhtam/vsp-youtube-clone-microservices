import { LibraryComponent } from './library/library.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LibraryRoutingModule } from './library-routing.module';
import { SharedModule } from '../shared/shared.module';
import { CoreModule } from '../core/core.module';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { LibraryVideosFeedComponent } from './library/library-videos-feed/library-videos-feed.component';
import { LibraryPlaylistsFeedComponent } from './library/library-playlists-feed/library-playlists-feed.component';

@NgModule({
  declarations: [
    LibraryComponent,
    LibraryVideosFeedComponent,
    LibraryPlaylistsFeedComponent,
  ],
  imports: [
    CommonModule,
    CoreModule,
    SharedModule,
    LibraryRoutingModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
})
export class LibraryModule {}
