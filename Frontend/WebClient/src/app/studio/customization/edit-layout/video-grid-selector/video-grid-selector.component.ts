import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Video } from 'src/app/core/models/channel';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-video-grid-selector',
  templateUrl: './video-grid-selector.component.html',
  styleUrls: ['./video-grid-selector.component.css'],
})
export class VideoGridSelectorComponent implements OnInit {
  @Input() video!: Video;
  @Output() selected = new EventEmitter();

  constructor() {}

  ngOnInit(): void {}

  get title() {
    return this.video.title;
  }

  get thumbnailUrl() {
    return this.video.thumbnailUrl || environment.assetSetup.noThumbnailIconUrl;
  }

  select() {
    this.selected.emit();
  }
}
