import { AuthService } from 'src/app/auth/services/auth.service';
import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import * as moment from 'moment';
import { map, of } from 'rxjs';
import { PlaylistAction } from 'src/app/core/actions';
import { AddToPlaylistDialogService } from 'src/app/shared/add-to-playlist-dialog/add-to-playlist-dialog.service';
import { isVideoAvailable, PlaylistItem } from 'src/app/core/models/library';
import { VideoVisibility } from 'src/app/core/models/video';
import { environment } from 'src/environments/environment';
import { getChannelLink } from 'src/app/core/services/utilities';
import { selectPlaylistState } from '../../../../core/selectors';

@Component({
  selector: 'app-playlist-item',
  templateUrl: './playlist-item.component.html',
  styleUrls: ['./playlist-item.component.css'],
})
export class PlaylistItemComponent implements OnInit {
  @Input()
  playlistId!: string;

  @Input()
  playlistItem!: PlaylistItem;

  @Input()
  movable!: boolean;

  @Input()
  isReadonly: boolean = true;

  @Input()
  index: number | null = null;

  focusing = false;

  constructor(
    private router: Router,
    private store: Store,
    private addToPlaylistDialog: AddToPlaylistDialogService,
    private authService:AuthService
  ) {}

  ngOnInit(): void {}

  openOptionsMenu() {
    this.focusing = true;
    return false;
  }

  optionsMenuClosed() {
    this.focusing = false;
  }

  get isHidden() {
    return !isVideoAvailable(this.playlistItem.video);
  }

  get title() {
    if (!isVideoAvailable(this.playlistItem.video)) {
      if (this.playlistItem.video.visibility === VideoVisibility.Private) {
        return '[Private video]';
      } else {
        return '[Deleted video]';
      }
    }

    return this.playlistItem.video.title;
  }

  get length() {
    if (isVideoAvailable(this.playlistItem.video)) {
      const lengthSeconds = this.playlistItem.video?.lengthSeconds ?? 0;
      if (lengthSeconds > 3600) {
        return new Date(lengthSeconds * 1000).toISOString().slice(11, 19);
      } else {
        return new Date(lengthSeconds * 1000).toISOString().slice(14, 19);
      }
    }

    return 0;
  }

  get creatorDisplayName() {
    if (isVideoAvailable(this.playlistItem.video)) {
      return this.playlistItem.video.creatorProfile.displayName;
    }

    return '';
  }

  get viewsCount() {
    if (isVideoAvailable(this.playlistItem.video)) {
      return this.playlistItem.video.metrics.viewsCount;
    }

    return 0;
  }

  get createDate() {
    if (isVideoAvailable(this.playlistItem.video)) {
      return moment(this.playlistItem.video.createDate).fromNow();
    }

    return '';
  }

  get thumbnailUrl() {
    if (isVideoAvailable(this.playlistItem.video)) {
      return (
        this.playlistItem.video.thumbnailUrl ||
        environment.assetSetup.noThumbnailIconUrl
      );
    }

    return environment.assetSetup.noThumbnailIconUrl;
  }

  get playlistTitle$() {
    switch (this.playlistId.toUpperCase()) {
      case 'WL':
        return of('Watch later');
      case 'LL':
        return of('Liked videos');
      case 'DL':
        return of('Disliked videos');
    }

    return this.store
      .select(selectPlaylistState)
      .pipe(map((x) => x.playlistInfo?.title));
  }

  get channelLink() {
    if (isVideoAvailable(this.playlistItem.video)) {
      return getChannelLink(this.playlistItem.video.creatorProfile);
    }

    return [];
  }

  remove() {
    const playlistId = this.playlistId.toUpperCase();

    if (playlistId == 'LL' || playlistId == 'WL') {
      this.store.dispatch(
        PlaylistAction.removePlaylistVideo({
          itemId: this.playlistItem.id,
        })
      );
    } else {
      this.store.dispatch(
        PlaylistAction.removePlaylistItem({ itemId: this.playlistItem.id })
      );
    }
  }

  moveToTop() {
    this.store.dispatch(
      PlaylistAction.movePlaylistItemToTop({ itemId: this.playlistItem.id })
    );
  }

  moveToBottom() {
    this.store.dispatch(
      PlaylistAction.movePlaylistItemToBottom({ itemId: this.playlistItem.id })
    );
  }

  addToPlaylist() {
    this.addToPlaylistDialog.openAddToPlaylistDialog(
      this.playlistItem.video.id,
      this.playlistItem.id
    );
  }
}
