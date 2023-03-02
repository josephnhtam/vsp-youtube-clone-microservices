import {Component, Input, OnInit} from '@angular/core';
import {TimePeriod, VideoGroup} from '../subscriptions.component';

@Component({
  selector: 'app-subscriptions-feed',
  templateUrl: './subscriptions-feed.component.html',
  styleUrls: ['./subscriptions-feed.component.css'],
})
export class SubscriptionsFeedComponent implements OnInit {
  @Input()
  videoGroup!: VideoGroup;

  @Input()
  isTop = false;

  title: string = '';

  constructor() {}

  ngOnInit(): void {
    switch (this.videoGroup.period) {
      case TimePeriod.today:
        this.title = 'Today';
        break;
      case TimePeriod.yesterday:
        this.title = 'Yesterday';
        break;
      case TimePeriod.week:
        this.title = 'This week';
        break;
      case TimePeriod.month:
        this.title = 'This month';
        break;
      case TimePeriod.older:
        this.title = 'Older';
        break;
    }
  }
}
