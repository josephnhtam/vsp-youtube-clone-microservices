import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { PlaylistInfo } from 'src/app/core/models/channel';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-playlists-browser-item',
  templateUrl: './playlists-browser-item.component.html',
  styleUrls: ['./playlists-browser-item.component.css'],
})
export class PlaylistsBrowserItemComponent implements OnInit {
  @Input() playlistInfo!: PlaylistInfo;
  @Input() isSelected!: boolean;
  @Output() selectionChange = new EventEmitter<boolean>();

  set selected(value: boolean) {
    this.isSelected = value;
    this.selectionChange.emit(value);
  }

  get selected() {
    return this.isSelected;
  }

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
