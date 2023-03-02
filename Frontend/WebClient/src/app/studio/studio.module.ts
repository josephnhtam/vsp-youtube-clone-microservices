import { CustomizationEffects } from './customization/effects/index';
import { CoreModule } from './../core/core.module';
import { EffectsModule } from '@ngrx/effects';
import { StoreModule } from '@ngrx/store';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { StudioRoutingModule } from './studio-routing.module';
import { SharedModule } from '../shared/shared.module';
import { MatDialogModule } from '@angular/material/dialog';
import { EditVideoDialogService } from './services/edit-video-dialog.service';
import { DragAndDrogFileDirective } from './directives/drag-and-drog-file.directive';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { UploadVideoComponent } from './edit-video-dialog/upload-video/upload-video.component';
import { MatStepperModule } from '@angular/material/stepper';
import { EditVideoDetailsComponent } from './edit-video-dialog/edit-video/edit-video-details/edit-video-details.component';
import { EditVideoVisibilityComponent } from './edit-video-dialog/edit-video/edit-video-visibility/edit-video-visibility.component';
import { MatInputModule } from '@angular/material/input';
import { MatRadioModule } from '@angular/material/radio';
import {
  MAT_TOOLTIP_DEFAULT_OPTIONS,
  MatTooltipDefaultOptions,
  MatTooltipModule,
} from '@angular/material/tooltip';
import { VideosManagementEffect } from './videos-management/effects';
import { UploaderEffects } from './uploader/effects';
import { studioFeatureKey, studioReducer } from './reducers';
import { StudioHubClientService } from './services/studio-hub-client.service';
import { StudioComponent } from './studio.component';
import { NgScrollbarModule } from 'ngx-scrollbar';
import { SidebarComponent } from './sidebar/sidebar.component';
import { MiniSidebarComponent } from './mini-sidebar/mini-sidebar.component';
import { VideosManagementComponent } from './videos-management/videos-management.component';
import { ToolsComponent } from './tools/tools.component';
import { MatMenuModule } from '@angular/material/menu';
import { StudioHubClientConnectorService } from './services/studio-hub-client-connector.service';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { EditVideoComponent } from './edit-video-dialog/edit-video/edit-video.component';
import { EditVideoDialogComponent } from './edit-video-dialog/edit-video-dialog.component';
import { MatSortModule } from '@angular/material/sort';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatChipsModule } from '@angular/material/chips';
import { MatTabsModule } from '@angular/material/tabs';
import { CustomizationComponent } from './customization/customization.component';
import { EditBrandingComponent } from './customization/edit-branding/edit-branding.component';
import { EditBasicInfoComponent } from './customization/edit-basic-info/edit-basic-info.component';
import { ImageUploaderComponent } from './customization/image-uploader/image-uploader.component';
import { UserDataResolver } from './customization/user-data-resolver.service';
import { EditLayoutComponent } from './customization/edit-layout/edit-layout.component';
import { ChannelSectionPreviewsListComponent } from './customization/edit-layout/channel-section-previews-list/channel-section-previews-list.component';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { ChannelSectionPreviewBaseComponent } from './customization/edit-layout/channel-section-preview-base/channel-section-preview-base.component';
import { ChannelVideosSectionPreviewComponent } from './customization/edit-layout/channel-videos-section-preview/channel-videos-section-preview.component';
import { ChannelCreatedPlaylistsSectionPreviewComponent } from './customization/edit-layout/channel-created-playlists-section-preview/channel-created-playlists-section-preview.component';
import { VideoPreviewComponent } from './customization/edit-layout/video-preview/video-preview.component';
import { PlaylistPreviewComponent } from './customization/edit-layout/playlist-preview/playlist-preview.component';
import { EditSinglePlaylistDialogComponent } from './customization/edit-layout/edit-single-playlist-dialog/edit-single-playlist-dialog.component';
import { EditSinglePlaylistDialogService } from './customization/edit-layout/edit-single-playlist-dialog/edit-single-playlist-dialog.service';
import { PlaylistGridSelectorComponent } from './customization/edit-layout/playlist-grid-selector/playlist-grid-selector.component';
import { ChannelSinglePlaylistSectionPreviewComponent } from './customization/edit-layout/channel-single-playlist-section-preview/channel-single-playlist-section-preview.component';
import { EditMultiplePlaylistsDialogComponent } from './customization/edit-layout/edit-multiple-playlists-dialog/edit-multiple-playlists-dialog.component';
import { PlaylistsBrowserComponent } from './customization/edit-layout/edit-multiple-playlists-dialog/playlists-browser/playlists-browser.component';
import { PlaylistsInSectionComponent } from './customization/edit-layout/edit-multiple-playlists-dialog/playlists-in-section/playlists-in-section.component';
import { PlaylistsBrowserItemComponent } from './customization/edit-layout/edit-multiple-playlists-dialog/playlists-browser/playlists-browser-item/playlists-browser-item.component';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { ChannelMultiplePlaylistsSectionPreviewComponent } from './customization/edit-layout/channel-multiple-playlists-section-preview/channel-multiple-playlists-section-preview.component';
import { EditVideoSpotlightComponent } from './customization/edit-layout/edit-video-spotlight/edit-video-spotlight.component';
import { ChooseVideoDialogComponent } from './customization/edit-layout/choose-video-dialog/choose-video-dialog.component';
import { VideoSpotlightComponent } from './customization/edit-layout/edit-video-spotlight/video-spotlight/video-spotlight.component';
import { VideoGridSelectorComponent } from './customization/edit-layout/video-grid-selector/video-grid-selector.component';
import { UploaderDialogComponent } from './uploader/uploader-dialog/uploader-dialog.component';
import { UploaderDialogService } from './uploader/uploader-dialog/uploader-dialog.service';
import { UploadProcessRowComponent } from './uploader/uploader-dialog/upload-process-row/upload-process-row.component';
import { MatSidenavModule } from '@angular/material/sidenav';
import { VideoPlayerModule } from '../video-player/video-player.module';

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
    EditVideoComponent,
    DragAndDrogFileDirective,
    UploadVideoComponent,
    EditVideoDialogComponent,
    EditVideoDetailsComponent,
    EditVideoVisibilityComponent,
    StudioComponent,
    SidebarComponent,
    MiniSidebarComponent,
    VideosManagementComponent,
    ToolsComponent,
    CustomizationComponent,
    EditBrandingComponent,
    EditBasicInfoComponent,
    ImageUploaderComponent,
    EditLayoutComponent,
    ChannelSectionPreviewsListComponent,
    ChannelSectionPreviewBaseComponent,
    ChannelVideosSectionPreviewComponent,
    ChannelCreatedPlaylistsSectionPreviewComponent,
    VideoPreviewComponent,
    PlaylistPreviewComponent,
    EditSinglePlaylistDialogComponent,
    PlaylistGridSelectorComponent,
    ChannelSinglePlaylistSectionPreviewComponent,
    EditMultiplePlaylistsDialogComponent,
    PlaylistsBrowserComponent,
    PlaylistsInSectionComponent,
    PlaylistsBrowserItemComponent,
    ChannelMultiplePlaylistsSectionPreviewComponent,
    EditVideoSpotlightComponent,
    ChooseVideoDialogComponent,
    VideoSpotlightComponent,
    VideoGridSelectorComponent,
    UploaderDialogComponent,
    UploadProcessRowComponent,
  ],
  providers: [
    EditVideoDialogService,
    StudioHubClientService,
    StudioHubClientConnectorService,
    UserDataResolver,
    EditSinglePlaylistDialogService,
    UploaderDialogService,
    matToolTipOptionProvider,
  ],
  imports: [
    CommonModule,
    CoreModule,
    SharedModule,
    StudioRoutingModule,
    VideoPlayerModule,
    NgScrollbarModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    MatStepperModule,
    MatInputModule,
    MatRadioModule,
    MatTooltipModule,
    MatMenuModule,
    MatTableModule,
    MatPaginatorModule,
    MatProgressSpinnerModule,
    MatSortModule,
    MatProgressBarModule,
    MatChipsModule,
    MatTabsModule,
    MatCheckboxModule,
    DragDropModule,
    MatSidenavModule,
    StoreModule.forFeature(studioFeatureKey, studioReducer),
    EffectsModule.forFeature([
      VideosManagementEffect,
      CustomizationEffects,
      UploaderEffects,
    ]),
  ],
})
export class StudioModule {}
