import {CdkDragDrop, moveItemInArray} from '@angular/cdk/drag-drop';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {SimplePlaylistInfo} from 'src/app/core/models/channel';

@Component({
  selector: 'app-playlists-in-section',
  templateUrl: './playlists-in-section.component.html',
  styleUrls: ['./playlists-in-section.component.css'],
})
export class PlaylistsInSectionComponent implements OnInit {
  @Input() playlistsInSection!: SimplePlaylistInfo[];
  @Output() playlistsInSectionChange = new EventEmitter<SimplePlaylistInfo[]>();

  constructor() {}

  ngOnInit(): void {}

  drop(event: CdkDragDrop<SimplePlaylistInfo[]>) {
    moveItemInArray(
      this.playlistsInSection,
      event.previousIndex,
      event.currentIndex
    );
    this.playlistsInSectionChange.emit(this.playlistsInSection);
  }

  remove(playlistId: string) {
    this.playlistsInSection = this.playlistsInSection.filter(
      (x) => x.id != playlistId
    );
    this.playlistsInSectionChange.emit(this.playlistsInSection);
  }
}
