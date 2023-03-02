import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {PlaylistInfo} from 'src/app/shared/playlist/playlist-grid-item/playlist-grid-item.component';

@Component({
  selector: 'app-library-playlists-feed',
  templateUrl: './library-playlists-feed.component.html',
  styleUrls: ['./library-playlists-feed.component.css'],
})
export class LibraryPlaylistsFeedComponent implements OnInit {
  @Input() playlistInfos: PlaylistInfo[] = [];
  @Input() hasMore = false;
  @Input() loadingMore = false;
  @Output() onLoadMore = new EventEmitter<void>();

  constructor() {}

  ngOnInit(): void {}

  loadMore() {
    this.onLoadMore.emit();
  }
}
