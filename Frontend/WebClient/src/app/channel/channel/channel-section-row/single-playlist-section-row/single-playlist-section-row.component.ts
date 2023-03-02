import {Router} from '@angular/router';
import {Component, Input, OnInit} from '@angular/core';
import {SinglePlaylistSectionInstance, Video,} from 'src/app/core/models/channel';

@Component({
  selector: 'app-single-playlist-section-row',
  templateUrl: './single-playlist-section-row.component.html',
  styleUrls: ['./single-playlist-section-row.component.css'],
})
export class SinglePlaylistSectionRowComponent implements OnInit {
  @Input() section!: SinglePlaylistSectionInstance;

  constructor(private router: Router) {}

  ngOnInit(): void {}

  get videos() {
    return this.section.playlist.items
      .map((x) => x.video)
      .filter((video): video is Video => {
        return 'title' in video;
      });
  }

  get title() {
    return this.section.playlist.title;
  }

  get description() {
    return this.section.playlist.description?.trim();
  }

  canPlayAll() {
    return !!this.videoId;
  }

  get videoId() {
    const video = this.section.playlist.items.find(
      (x) => 'title' in x.video
    )?.video;

    return video?.id;
  }

  get playlistId() {
    return this.section.playlist.id;
  }
}
