import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { HistoryRoutingModule } from './history-routing.module';
import { HistoryComponent } from './history/history.component';
import { HistoryHeaderComponent } from './history/history-header/history-header.component';
import { UserWatchRecordRowComponent } from './history/user-watch-history/user-watch-record-row/user-watch-record-row.component';
import { UserWatchHistoryComponent } from './history/user-watch-history/user-watch-history.component';
import { HistoryGuard } from './history/history-guard.service';
import { UserHistorySettingsService } from './history/user-history-settings.service';
import { CoreModule } from '../core/core.module';
import { SharedModule } from '../shared/shared.module';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { NgScrollbarModule } from 'ngx-scrollbar';
import { MatMenuModule } from '@angular/material/menu';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatBadgeModule } from '@angular/material/badge';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@NgModule({
  declarations: [
    HistoryComponent,
    HistoryHeaderComponent,
    UserWatchRecordRowComponent,
    UserWatchHistoryComponent,
  ],
  providers: [HistoryGuard, UserHistorySettingsService],
  imports: [
    CommonModule,
    CoreModule,
    SharedModule,
    HistoryRoutingModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
    NgScrollbarModule,
    MatMenuModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
  ],
})
export class HistoryModule {}
