import {getChannelLink} from 'src/app/core/services/utilities';
import {UserProfileService} from './../../core/services/user-profile.service';
import {Component, Input, OnInit} from '@angular/core';
import {SidebarService} from './sidebar.service';
import {map} from 'rxjs';
import {UserProfile} from "../../core/models/subscription";

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css'],
})
export class SidebarComponent implements OnInit {
  @Input() miniSidebar = false;

  leastPlaylistInfosShown = 1;
  leastSubscriptionInfosShown = 7;

  showMorePlaylists = false;
  showMoreSubscriptions = false;

  constructor(
    private userProfileService: UserProfileService,
    private service: SidebarService
  ) {}

  ngOnInit(): void {}

  get isUserReady$() {
    return this.userProfileService.userReady$;
  }

  get canShowMorePlaylistInfos$() {
    return this.service.playlistInfos$.pipe(
      map((infos) => infos.length > this.leastPlaylistInfosShown)
    );
  }

  get hiddenPlaylistInfosCount$() {
    return this.service.playlistInfos$.pipe(
      map((infos) => infos.length - this.leastPlaylistInfosShown)
    );
  }

  get canShowMoreSubscriptionInfos$() {
    return this.service.subscriptionInfos$.pipe(
      map((infos) => infos.length > this.leastSubscriptionInfosShown)
    );
  }

  get hiddenSubscriptionInfosCount$() {
    return this.service.subscriptionInfos$.pipe(
      map((infos) => infos.length - this.leastSubscriptionInfosShown)
    );
  }

  get playlistInfos$() {
    return this.service.playlistInfos$.pipe(
      map((infos) => {
        if (!this.showMorePlaylists) {
          return infos.slice(0, this.leastPlaylistInfosShown);
        } else {
          return infos;
        }
      })
    );
  }

  get subscriptionInfos$() {
    return this.service.subscriptionInfos$.pipe(
      map((infos) => {
        if (!this.showMoreSubscriptions) {
          return infos.slice(0, this.leastSubscriptionInfosShown);
        } else {
          return infos;
        }
      })
    );
  }

  getUserChannelLink(userProfile: UserProfile) {
    return getChannelLink(userProfile);
  }
}
