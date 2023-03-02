import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';

@Component({
  selector: 'app-edit-video-spotlight',
  templateUrl: './edit-video-spotlight.component.html',
  styleUrls: ['./edit-video-spotlight.component.css'],
})
export class EditVideoSpotlightComponent implements OnInit {
  @Input() unsubscribedSpotlightVideoId: string | null = null;
  @Input() subscribedSpotlightVideoId: string | null = null;
  @Output() unsubscribedSpotlightVideoIdChange = new EventEmitter<
    string | null
  >();
  @Output() subscribedSpotlightVideoIdChange = new EventEmitter<
    string | null
  >();

  constructor() {}

  ngOnInit(): void {}

  setUnsubscribedSpotlightVideoId(videoId: string | null) {
    console.log(videoId);
    this.unsubscribedSpotlightVideoId = videoId;
    this.unsubscribedSpotlightVideoIdChange.emit(videoId);
  }

  setSubscribedSpotlightVideoId(videoId: string | null) {
    this.subscribedSpotlightVideoId = videoId;
    this.subscribedSpotlightVideoIdChange.emit(videoId);
  }
}
