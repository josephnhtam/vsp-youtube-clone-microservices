import { isVideoAvailable } from 'src/app/core/models/library';
import { Component, Input } from '@angular/core';
import { HiddenVideo, Video } from 'src/app/core/models/channel';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-video-preview',
  templateUrl: './video-preview.component.html',
  styleUrls: ['./video-preview.component.css'],
})
export class VideoPreviewComponent {
  @Input()
  video!: Video | HiddenVideo;

  get length() {
    if (isVideoAvailable(this.video)) {
      const lengthSeconds = this.video?.lengthSeconds ?? 0;
      if (lengthSeconds > 3600) {
        return new Date(lengthSeconds * 1000).toISOString().slice(11, 19);
      } else {
        return new Date(lengthSeconds * 1000).toISOString().slice(14, 19);
      }
    }

    return 0;
  }

  get thumbnailUrl() {
    if (this.isVideoAvailable(this.video)) {
      return (
        this.video.thumbnailUrl || environment.assetSetup.noThumbnailIconUrl
      );
    } else {
      return environment.assetSetup.noThumbnailIconUrl;
    }
  }

  isVideoAvailable(video: Video | HiddenVideo): video is Video {
    return 'title' in video;
  }
}
