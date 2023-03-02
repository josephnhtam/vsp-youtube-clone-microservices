import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import * as moment from 'moment';
import { CreatorProfile, VideoMetrics } from 'src/app/core/models/search';
import { environment } from 'src/environments/environment';
import { AddToPlaylistDialogService } from '../../add-to-playlist-dialog/add-to-playlist-dialog.service';
import { getChannelLink } from '../../../core/services/utilities';

@Component({
  selector: 'app-video-row',
  templateUrl: './video-row.component.html',
  styleUrls: ['./video-row.component.css'],
})
export class VideoRowComponent implements OnInit {
  @Input() video?: Video;
  @Input() inlineMetadata = false;

  focusing = false;
  mouseover = false;
  previewThumbnailLoaded = false;

  private mouseoverTimer: any;

  constructor(
    private router: Router,
    private addToPlaylistDialog: AddToPlaylistDialogService
  ) {}

  ngOnInit(): void {}

  get length() {
    const lengthSeconds = this.video?.lengthSeconds ?? 0;
    if (lengthSeconds > 3600) {
      return new Date(lengthSeconds * 1000).toISOString().slice(11, 19);
    } else {
      return new Date(lengthSeconds * 1000).toISOString().slice(14, 19);
    }
  }

  get isPlaceholder() {
    return !this.video;
  }

  get createDate() {
    return moment(this.video?.createDate).fromNow();
  }

  get thumbnailUrl() {
    return (
      this.video?.thumbnailUrl || environment.assetSetup.noThumbnailIconUrl
    );
  }

  get channelLink() {
    return getChannelLink(this.video?.creatorProfile);
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
}

export interface Video {
  id: string;
  title: string;
  creatorProfile: CreatorProfile;
  description: string | null;
  thumbnailUrl: string | null;
  previewThumbnailUrl: string | null;
  lengthSeconds: number | null;
  metrics: VideoMetrics;
  createDate: string;
}
