import {VideosSectionInstance} from '../../../../core/models/channel';
import {Component, Input, OnInit} from '@angular/core';

@Component({
  selector: 'app-videos-section-row',
  templateUrl: './videos-section-row.component.html',
  styleUrls: ['./videos-section-row.component.css'],
})
export class VideosSectionRowComponent implements OnInit {
  @Input() userId!: string;
  @Input() section!: VideosSectionInstance;

  ngOnInit(): void {}

  canPlayAll() {
    return this.section.videos.length > 1;
  }

  get videoId() {
    return this.section.videos[0].id;
  }

  get playlistId() {
    return `videos-${this.userId}`;
  }
}
