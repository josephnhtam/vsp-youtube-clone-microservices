import {
  isVideoAvailable,
  UserWatchRecord,
} from '../../../../core/models/history';
import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AddToPlaylistDialogService } from 'src/app/shared/add-to-playlist-dialog/add-to-playlist-dialog.service';
import * as moment from 'moment';
import { Store } from '@ngrx/store';
import { HistoryAction } from 'src/app/core/actions';
import { environment } from 'src/environments/environment';
import { VideoVisibility } from 'src/app/core/models/video';
import { getChannelLink } from 'src/app/core/services/utilities';

@Component({
  selector: 'app-user-watch-record-row',
  templateUrl: './user-watch-record-row.component.html',
  styleUrls: ['./user-watch-record-row.component.css'],
})
export class UserWatchRecordRowComponent implements OnInit {
  @Input()
  record!: UserWatchRecord;

  focusing = false;
  mouseover = false;
  previewThumbnailLoaded = false;

  private mouseoverTimer: any;

  constructor(
    private store: Store,
    private router: Router,
    private addToPlaylistDialog: AddToPlaylistDialogService
  ) {}

  ngOnInit(): void {}

  get video() {
    return this.record.video;
  }

  get isHidden() {
    return !isVideoAvailable(this.video);
  }

  get title() {
    if (!isVideoAvailable(this.video)) {
      if (this.video.visibility === VideoVisibility.Private) {
        return '[Private video]';
      } else {
        return '[Deleted video]';
      }
    }

    return this.video.title;
  }

  get length() {
    if (isVideoAvailable(this.video)) {
      const lengthSeconds = this.video.lengthSeconds ?? 0;
      if (lengthSeconds > 3600) {
        return new Date(lengthSeconds * 1000).toISOString().slice(11, 19);
      } else {
        return new Date(lengthSeconds * 1000).toISOString().slice(14, 19);
      }
    }

    return 0;
  }

  get isPlaceholder() {
    return !this.record;
  }

  get createDate() {
    if (isVideoAvailable(this.video)) {
      return moment(this.video.createDate).fromNow();
    }

    return '';
  }

  get channelLink() {
    return getChannelLink(this.creatorProfile);
  }

  get thumbnailUrl() {
    if (isVideoAvailable(this.video)) {
      return (
        this.video.thumbnailUrl || environment.assetSetup.noThumbnailIconUrl
      );
    }

    return environment.assetSetup.noThumbnailIconUrl;
  }

  get previewThumbnailUrl() {
    if (isVideoAvailable(this.video)) {
      return this.video.previewThumbnailUrl;
    }

    return null;
  }

  get creatorProfile() {
    if (isVideoAvailable(this.video)) {
      return this.video.creatorProfile;
    }

    return undefined;
  }

  get viewsCount() {
    if (isVideoAvailable(this.video)) {
      return this.video.metrics.viewsCount;
    }

    return 0;
  }

  get description() {
    if (isVideoAvailable(this.video)) {
      return this.video.description;
    }

    return '';
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

  removeVideo() {
    if (this.video) {
      this.store.dispatch(
        HistoryAction.removeVideo({ videoId: this.video.id })
      );
    }
  }
}
