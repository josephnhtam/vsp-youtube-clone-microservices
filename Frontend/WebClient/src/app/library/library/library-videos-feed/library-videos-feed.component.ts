import {Component, Input, OnInit} from '@angular/core';
import {HiddenVideo, Video,} from 'src/app/shared/video/video-grid-item/video-grid-item.component';

@Component({
  selector: 'app-library-videos-feed',
  templateUrl: './library-videos-feed.component.html',
  styleUrls: ['./library-videos-feed.component.css'],
})
export class LibraryVideosFeedComponent implements OnInit {
  @Input()
  videos: (Video | HiddenVideo)[] = [];

  constructor() {}

  ngOnInit(): void {}
}
