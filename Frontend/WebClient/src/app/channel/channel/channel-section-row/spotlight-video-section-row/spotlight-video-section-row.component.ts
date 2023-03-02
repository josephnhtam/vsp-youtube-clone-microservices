import {SpotlightVideoSectionInstance} from '../../../../core/models/channel';
import {Component, Input, OnInit} from '@angular/core';

@Component({
  selector: 'app-spotlight-video-section-row',
  templateUrl: './spotlight-video-section-row.component.html',
  styleUrls: ['./spotlight-video-section-row.component.css'],
})
export class SpotlightVideoSectionRowComponent implements OnInit {
  @Input() section!: SpotlightVideoSectionInstance;

  constructor() {}

  ngOnInit(): void {}

  get video() {
    return this.section.video;
  }
}
