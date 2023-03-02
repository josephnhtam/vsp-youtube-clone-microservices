import { AddToPlaylistDialogService } from 'src/app/shared/add-to-playlist-dialog/add-to-playlist-dialog.service';
import { isVideoAvailable, PlaylistItem } from 'src/app/core/models/library';
import {
  Component,
  Input,
  OnChanges,
  OnInit,
  SimpleChanges,
} from '@angular/core';
import { Router } from '@angular/router';
import { VideoVisibility } from 'src/app/core/models/video';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-video-playlist-item',
  templateUrl: './video-playlist-item.component.html',
  styleUrls: ['./video-playlist-item.component.css'],
})
export class VideoPlaylistItemComponent implements OnInit, OnChanges {
  @Input()
  item!: PlaylistItem;

  @Input()
  isCurrent: boolean = false;

  focusing = false;

  constructor(
    private router: Router,
    private addToPlaylistDialog: AddToPlaylistDialogService
  ) {}

  ngOnChanges(changes: SimpleChanges) {}

  openOptionsMenu() {
    this.focusing = true;
    return false;
  }

  optionsMenuClosed() {
    this.focusing = false;
  }

  get video() {
    return this.item.video;
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

  get position() {
    return this.item.position! + 1;
  }

  get creatorDisplayName() {
    if (isVideoAvailable(this.video)) {
      return this.video.creatorProfile.displayName;
    }

    return '';
  }

  get thumbnailUrl() {
    if (isVideoAvailable(this.video)) {
      return (
        this.video.thumbnailUrl || environment.assetSetup.noThumbnailIconUrl
      );
    }

    return environment.assetSetup.noThumbnailIconUrl;
  }

  addToPlaylist() {
    this.addToPlaylistDialog.openAddToPlaylistDialog(
      this.video.id,
      this.item.id
    );
  }

  get isHidden() {
    return !isVideoAvailable(this.video);
  }

  ngOnInit(): void {}
}
