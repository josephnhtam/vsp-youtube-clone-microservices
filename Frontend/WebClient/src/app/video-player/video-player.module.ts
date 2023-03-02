import { VgControlsModule } from '@videogular/ngx-videogular/controls';
import { VgBufferingModule } from '@videogular/ngx-videogular/buffering';
import { VgOverlayPlayModule } from '@videogular/ngx-videogular/overlay-play';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { VideoPlayerComponent } from './video-player/video-player.component';
import { VgCoreModule } from '@videogular/ngx-videogular/core';
import { QualitySelectorComponent } from './video-player/quality-selector/quality-selector.component';
import { ScrubBarComponent } from './video-player/scrub-bar/scrub-bar.component';

@NgModule({
  declarations: [
    VideoPlayerComponent,
    QualitySelectorComponent,
    ScrubBarComponent,
  ],
  imports: [
    CommonModule,
    VgCoreModule,
    VgControlsModule,
    VgOverlayPlayModule,
    VgBufferingModule,
  ],
  exports: [VideoPlayerComponent],
})
export class VideoPlayerModule {}
