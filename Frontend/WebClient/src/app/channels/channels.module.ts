import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChannelsRoutingModule } from './channels-routing.module';
import { ChannelsComponent } from './channels/channels.component';
import { ChannelRowComponent } from './channels/channel-row/channel-row.component';
import { CoreModule } from '../core/core.module';
import { SharedModule } from '../shared/shared.module';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@NgModule({
  declarations: [ChannelsComponent, ChannelRowComponent],
  imports: [
    CommonModule,
    CoreModule,
    SharedModule,
    ChannelsRoutingModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatProgressSpinnerModule,
  ],
})
export class ChannelsModule {}
