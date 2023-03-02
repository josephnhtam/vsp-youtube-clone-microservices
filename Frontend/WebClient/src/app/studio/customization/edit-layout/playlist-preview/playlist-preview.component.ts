import { Component, Input, OnInit } from '@angular/core';
import { PlaylistInfo } from 'src/app/core/models/channel';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-playlist-preview',
  templateUrl: './playlist-preview.component.html',
  styleUrls: ['./playlist-preview.component.css'],
})
export class PlaylistPreviewComponent implements OnInit {
  @Input() playlistInfo!: PlaylistInfo;

  constructor() {}

  ngOnInit(): void {}

  get thumbnailUrl() {
    return (
      this.playlistInfo.thumbnailUrl ||
      environment.assetSetup.noThumbnailIconUrl
    );
  }

  get itemsCount() {
    return this.playlistInfo.itemsCount;
  }
}
