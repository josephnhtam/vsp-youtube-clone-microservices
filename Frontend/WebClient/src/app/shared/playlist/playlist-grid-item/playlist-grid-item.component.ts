import { map } from 'rxjs';
import { AuthService } from 'src/app/auth/services/auth.service';
import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import * as moment from 'moment';
import { PlaylistVisibility } from 'src/app/core/models/library';
import { CreatorProfile } from 'src/app/core/models/search';
import { environment } from 'src/environments/environment';
import { getChannelLink } from '../../../core/services/utilities';

@Component({
  selector: 'app-playlist-grid-item',
  templateUrl: './playlist-grid-item.component.html',
  styleUrls: ['./playlist-grid-item.component.css'],
})
export class PlaylistGridItemComponent implements OnInit {
  @Input() playlistInfo?: PlaylistInfo;
  @Input() hideUserThumbnail = false;
  @Input() hideUser = false;
  @Input() hideUpdateDate = false;
  @Input() hideVisibility = false;

  focusing = false;
  mouseover = false;

  constructor(private router: Router, private authService: AuthService) {}

  ngOnInit(): void {}

  getPlaylistList() {
    return ['/playlist'];
  }

  getViewLink() {
    if (this.playlistInfo != null) {
      if (this.playlistInfo.videoId != null) {
        return ['/watch', this.playlistInfo.videoId];
      } else {
        return this.getPlaylistList();
      }
    }

    return [];
  }

  getQueryParams() {
    return this.getPlaylistQueryParams();
  }

  getPlaylistQueryParams() {
    if (this.playlistInfo != null) {
      return {
        list: this.playlistInfo.id,
      };
    } else {
      return {};
    }
  }

  get isMine$() {
    return this.authService.authInfo$.pipe(
      map((authInfo) => authInfo?.sub === this.playlistInfo?.creatorProfile.id)
    );
  }

  get isPlaceholder() {
    return !this.playlistInfo;
  }

  get title() {
    return this.playlistInfo?.title;
  }

  get visibilityIcon() {
    switch (this.playlistInfo?.visibility) {
      default:
      case PlaylistVisibility.Private:
        return 'lock';
      case PlaylistVisibility.Unlisted:
        return 'link';
      case PlaylistVisibility.Public:
        return 'public';
    }
  }

  get visibility() {
    switch (this.playlistInfo?.visibility) {
      default:
      case PlaylistVisibility.Private:
        return 'Private';
      case PlaylistVisibility.Unlisted:
        return 'Unlisted';
      case PlaylistVisibility.Public:
        return 'Public';
    }
  }

  get creatorProfile() {
    return this.playlistInfo?.creatorProfile;
  }

  get createDate() {
    return moment(this.playlistInfo?.createDate).fromNow();
  }

  get updateDate() {
    return moment(this.playlistInfo?.updateDate).fromNow();
  }

  get thumbnailUrl() {
    return (
      this.playlistInfo?.thumbnailUrl ||
      environment.assetSetup.noThumbnailIconUrl
    );
  }

  get itemsCount() {
    return this.playlistInfo?.itemsCount;
  }

  get channelLink() {
    return getChannelLink(this.creatorProfile);
  }

  mouseEnter() {
    this.mouseover = true;
  }

  mouseLeave() {
    this.mouseover = false;
  }
}

export interface PlaylistInfo {
  id: string;
  creatorProfile: CreatorProfile;
  title: string;
  thumbnailUrl: string | null;
  videoId: string | null;
  visibility: PlaylistVisibility | null;
  itemsCount: number;
  createDate: string;
  updateDate: string;
}
