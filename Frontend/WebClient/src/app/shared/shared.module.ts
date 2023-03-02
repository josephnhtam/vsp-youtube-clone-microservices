import { RouterModule } from '@angular/router';
import { CoreModule } from './../core/core.module';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { CommonToolbarComponent } from './common-toolbar/common-toolbar.component';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDialogModule } from '@angular/material/dialog';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { NgScrollbarModule } from 'ngx-scrollbar';
import { VideoGridItemComponent } from './video/video-grid-item/video-grid-item.component';
import { VideoRowComponent } from './video/video-row/video-row.component';
import { VideoRowSmallComponent } from './video/video-row-small/video-row-small.component';
import { UserThumbnailComponent } from './user-thumbnail/user-thumbnail.component';
import { UserProfileThumbnailComponent } from './user-profile-thumbnail/user-profile-thumbnail.component';
import { AddToPlaylistDialogService } from './add-to-playlist-dialog/add-to-playlist-dialog.service';
import { AddToPlaylistDialogComponent } from './add-to-playlist-dialog/add-to-playlist-dialog.component';
import { AddToPlaylistActionService } from './add-to-playlist-dialog/add-to-playlist-actions.service';
import { PlaylistGridItemComponent } from './playlist/playlist-grid-item/playlist-grid-item.component';
import { UserThumbnailToolComponent } from './user-thumbnail-tool/user-thumbnail-tool.component';
import { ConfirmDialogComponent } from './confirm-dialog/confirm-dialog.component';
import { ConfirmDialogService } from './confirm-dialog/confirm-dialog.service';
import { OverSidebarComponent } from './over-sidebar/over-sidebar.component';
import {
  MatTooltipDefaultOptions,
  MAT_TOOLTIP_DEFAULT_OPTIONS,
} from '@angular/material/tooltip';
import { CreateUserProfileDialogComponent } from './create-user-profile-dialog/create-user-profile-dialog.component';
import { ErrorDialogComponent } from './error-dialog/error-dialog.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { CreateUserProfileDialogService } from './create-user-profile-dialog/create-user-profile-dialog.service';
import { UserProfileResolver } from './guards/user-profile-resolver.service';

const tooltipOptions: MatTooltipDefaultOptions = {
  showDelay: 0,
  hideDelay: 0,
  touchendHideDelay: 0,
  disableTooltipInteractivity: true,
};

const matToolTipOptionProvider = {
  provide: MAT_TOOLTIP_DEFAULT_OPTIONS,
  useValue: tooltipOptions,
};

@NgModule({
  declarations: [
    CommonToolbarComponent,
    VideoGridItemComponent,
    VideoRowComponent,
    VideoRowSmallComponent,
    UserThumbnailComponent,
    UserProfileThumbnailComponent,
    AddToPlaylistDialogComponent,
    PlaylistGridItemComponent,
    UserThumbnailToolComponent,
    ConfirmDialogComponent,
    OverSidebarComponent,
    CreateUserProfileDialogComponent,
    ErrorDialogComponent,
  ],
  providers: [
    UserProfileResolver,
    AddToPlaylistDialogService,
    AddToPlaylistActionService,
    ConfirmDialogService,
    CreateUserProfileDialogService,
    matToolTipOptionProvider,
  ],
  imports: [
    CommonModule,
    CoreModule,
    FormsModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatCheckboxModule,
    NgScrollbarModule,
    MatSnackBarModule,
    RouterModule,
  ],
  exports: [
    FormsModule,
    ReactiveFormsModule,
    CommonToolbarComponent,
    VideoGridItemComponent,
    VideoRowComponent,
    VideoRowSmallComponent,
    PlaylistGridItemComponent,
    UserThumbnailComponent,
    UserProfileThumbnailComponent,
    AddToPlaylistDialogComponent,
    UserThumbnailToolComponent,
    OverSidebarComponent,
  ],
})
export class SharedModule {}
