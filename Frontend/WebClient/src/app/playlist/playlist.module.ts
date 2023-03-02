import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PlaylistRoutingModule } from './playlist-routing.module';
import { PlaylistComponent } from './playlist/playlist.component';
import { PlaylistResolver } from './playlist/playlist-resolver.service';
import { PlaylistHeaderComponent } from './playlist/playlist-header/playlist-header.component';
import { PlaylistContentComponent } from './playlist/playlist-content/playlist-content.component';
import { PlaylistItemComponent } from './playlist/playlist-content/playlist-item/playlist-item.component';
import { CoreModule } from '../core/core.module';
import { SharedModule } from '../shared/shared.module';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { NgScrollbarModule } from 'ngx-scrollbar';
import { MatMenuModule } from '@angular/material/menu';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { DragDropModule } from '@angular/cdk/drag-drop';

@NgModule({
  declarations: [
    PlaylistComponent,
    PlaylistHeaderComponent,
    PlaylistContentComponent,
    PlaylistItemComponent,
  ],
  providers: [PlaylistResolver],
  imports: [
    CommonModule,
    CoreModule,
    SharedModule,
    MatButtonModule,
    MatIconModule,
    NgScrollbarModule,
    MatMenuModule,
    MatInputModule,
    MatProgressSpinnerModule,
    DragDropModule,
    PlaylistRoutingModule,
  ],
})
export class PlaylistModule {}
