import { VideoVisibility } from '../../../core/models/video';
import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CreatorProfile, VideoMetrics } from 'src/app/core/models/search';
import * as moment from 'moment';
import { AddToPlaylistDialogService } from '../../add-to-playlist-dialog/add-to-playlist-dialog.service';
import { environment } from 'src/environments/environment';
import { getChannelLink } from '../../../core/services/utilities';

@Component({
  selector: 'app-video-grid-item',
  templateUrl: './video-grid-item.component.html',
  styleUrls: ['./video-grid-item.component.css'],
})
export class VideoGridItemComponent implements OnInit {
  @Input()
  video?: Video | HiddenVideo;

  @Input()
  hideUserThumbnail = false;

  @Input()
  hideUser = false;

  focusing = false;
  mouseover = false;
  previewThumbnailLoaded = false;

  private mouseoverTimer: any;

  constructor(
    private router: Router,
    private addToPlaylistDialog: AddToPlaylistDialogService
  ) {}

  ngOnInit(): void {}

  get isPlaceholder() {
    return !this.video;
  }

  get isHidden() {
    return !isVideoAvailable(this.video!);
  }

  get title() {
    if (this.video && !isVideoAvailable(this.video)) {
      if (this.video.visibility === VideoVisibility.Private) {
        return '[Private video]';
      } else {
        return '[Deleted video]';
      }
    }

    return this.video?.title;
  }

  get length() {
    if (this.video && isVideoAvailable(this.video)) {
      const lengthSeconds = this.video?.lengthSeconds ?? 0;
      if (lengthSeconds > 3600) {
        return new Date(lengthSeconds * 1000).toISOString().slice(11, 19);
      } else {
        return new Date(lengthSeconds * 1000).toISOString().slice(14, 19);
      }
    }

    return 0;
  }

  get creatorProfile() {
    if (this.video && isVideoAvailable(this.video)) {
      return this.video.creatorProfile;
    }

    return undefined;
  }

  get viewsCount() {
    if (this.video && isVideoAvailable(this.video)) {
      return this.video.metrics.viewsCount;
    }

    return 0;
  }

  get createDate() {
    if (this.video && isVideoAvailable(this.video)) {
      return moment(this.video.createDate).fromNow();
    }

    return '';
  }

  get thumbnailUrl() {
    if (this.video && isVideoAvailable(this.video)) {
      return (
        this.video.thumbnailUrl || environment.assetSetup.noThumbnailIconUrl
      );
    }

    return environment.assetSetup.noThumbnailIconUrl;
  }

  get previewThumbnailUrl() {
    if (this.video && isVideoAvailable(this.video)) {
      return this.video.previewThumbnailUrl;
    }

    return null;
  }

  addToPlaylist() {
    if (this.video) {
      this.addToPlaylistDialog.openAddToPlaylistDialog(this.video.id);
    }
  }

  openOptionsMenu() {
    this.focusing = true;
    return false;
  }

  optionsMenuClosed() {
    this.focusing = false;
  }

  mouseEnter() {
    if (!this.mouseoverTimer) {
      this.mouseoverTimer = setTimeout(() => {
        this.mouseover = true;
      }, 300);
    }
  }

  mouseLeave() {
    clearTimeout(this.mouseoverTimer);
    this.mouseoverTimer = null;
    this.mouseover = false;
  }

  get channelLink() {
    return getChannelLink(this.creatorProfile);
  }
}

function isVideoAvailable(video: Video | HiddenVideo): video is Video {
  return !!video && 'title' in video;
}

export interface Video {
  id: string;
  title: string;
  creatorProfile: CreatorProfile;
  thumbnailUrl: string | null;
  previewThumbnailUrl: string | null;
  lengthSeconds: number | null;
  metrics: VideoMetrics;
  createDate: string;
}

export interface HiddenVideo {
  id: string;
  visibility: VideoVisibility | null;
}
