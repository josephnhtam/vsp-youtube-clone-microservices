import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { PlaylistInfo } from 'src/app/core/models/channel';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-playlist-grid-selector',
  templateUrl: './playlist-grid-selector.component.html',
  styleUrls: ['./playlist-grid-selector.component.css'],
})
export class PlaylistGridSelectorComponent implements OnInit {
  @Input() playlistInfo!: PlaylistInfo;
  @Output() selected = new EventEmitter();

  constructor() {}

  ngOnInit(): void {}

  get title() {
    return this.playlistInfo.title;
  }

  get thumbnailUrl() {
    return (
      this.playlistInfo.thumbnailUrl ||
      environment.assetSetup.noThumbnailIconUrl
    );
  }

  get itemsCount() {
    return this.playlistInfo.itemsCount;
  }

  select() {
    this.selected.emit();
  }
}
