import {ChannelHelperService} from 'src/app/core/services/channel-helper.service';
import {ActivatedRoute} from '@angular/router';
import {Component, OnInit} from '@angular/core';
import {ChannelData} from 'src/app/core/models/channel';

@Component({
  selector: 'app-channel-about',
  templateUrl: './channel-about.component.html',
  styleUrls: ['./channel-about.component.css'],
})
export class ChannelAboutComponent implements OnInit {
  channelData: ChannelData;

  viewsCount: number = 0;
  statsPending = false;

  constructor(
    private route: ActivatedRoute,
    private service: ChannelHelperService
  ) {
    this.channelData = route.parent?.snapshot.data[
      'channelData'
    ] as ChannelData;
  }

  ngOnInit(): void {
    this.refreshStats();
  }

  refreshStats() {
    this.statsPending = true;

    this.service
      .getTotalViewsCount(this.channelData.userProfile.id)
      .subscribe((result) => {
        this.viewsCount = result;
        this.statsPending = false;
      });
  }

  get description() {
    return this.channelData.userProfile.description;
  }

  get joinDate() {
    const date = new Date(this.channelData.userProfile.createDate);
    return `${
      monthNames[date.getMonth()]
    } ${date.getDate()}, ${date.getFullYear()}`;
  }
}

const monthNames = [
  'Jan',
  'Feb',
  'Mar',
  'Apr',
  'May',
  'Jun',
  'Jul',
  'Aug',
  'Sep',
  'Oct',
  'Nov',
  'Dec',
];
