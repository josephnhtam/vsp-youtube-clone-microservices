import { EffectsModule } from '@ngrx/effects';
import { communityReducer } from './reducers/index';
import { SharedModule } from './../shared/shared.module';
import { CoreModule } from './../core/core.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { VideoCommentBlockComponent } from './video-forum/video-comment-block/video-comment-block.component';
import { VideoCommentAdderComponent } from './video-forum/video-comment-adder/video-comment-adder.component';
import { ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { StoreModule } from '@ngrx/store';
import { communityFeatureKey } from './reducers';
import { VideoForumEffect } from './video-forum/effects/video-forum';
import { VideoForumComponent } from './video-forum/video-forum.component';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatMenuModule } from '@angular/material/menu';
import { VideoCommentEditorComponent } from './video-forum/video-comment-editor/video-comment-editor.component';

@NgModule({
  declarations: [
    VideoCommentBlockComponent,
    VideoCommentAdderComponent,
    VideoForumComponent,
    VideoCommentEditorComponent,
  ],
  imports: [
    CommonModule,
    CoreModule,
    SharedModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatMenuModule,
    StoreModule.forFeature(communityFeatureKey, communityReducer),
    EffectsModule.forFeature([VideoForumEffect]),
  ],
  exports: [
    VideoCommentBlockComponent,
    VideoCommentAdderComponent,
    VideoForumComponent,
  ],
})
export class CommunityModule {}
