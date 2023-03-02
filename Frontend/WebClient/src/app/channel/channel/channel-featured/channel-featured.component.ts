import {AuthService} from 'src/app/auth/services/auth.service';
import {
  selectChannelSectionInstances,
  selectIsChannelSectionsLoading,
} from './../../../core/selectors/channel-sections';
import {Store} from '@ngrx/store';
import {ChannelData, UserChannel,} from '../../../core/models/channel';
import {Component, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {map, of, Subscription, take} from 'rxjs';
import {Guid} from 'guid-typescript';
import {ChannelSectionAction} from 'src/app/core/actions';
import {AuthInfo} from 'src/app/auth/models/auth-info';
import {ChannelSubscriptionService} from 'src/app/core/services/channel-subscription.service';

@Component({
  selector: 'app-channel-featured',
  templateUrl: './channel-featured.component.html',
  styleUrls: ['./channel-featured.component.css'],
})
export class ChannelFeaturedComponent implements OnInit, OnDestroy {
  channel!: UserChannel;
  spotlightVideoId: string | null = null;

  private contextId: string | null = null;
  private channelSub?: Subscription;

  constructor(
    private route: ActivatedRoute,
    private store: Store,
    private authService: AuthService,
    private subscriptionService: ChannelSubscriptionService
  ) {
    this.channelSub = this.route.data
      .pipe(map((data) => data['channel'] as UserChannel))
      .subscribe((channel) => {
        this.channel = channel;
        this.onChannelRefreshed();
      });
  }

  ngOnDestroy(): void {
    this.channelSub?.unsubscribe();
  }

  ngOnInit(): void {}

  onChannelRefreshed() {
    this.contextId = null;

    this.instantiate();
  }

  instantiate() {
    this.contextId = Guid.create().toString();

    this.authService.authInfo$.pipe(take(1)).subscribe((authInfo) => {
      this.refreshSpotlightVideoId(authInfo);

      this.store.dispatch(
        ChannelSectionAction.instantiate({
          userId: this.channel.id,
          sections: this.channel.sections,
          contextId: this.contextId!,
        })
      );
    });
  }

  private refreshSpotlightVideoId(authInfo: AuthInfo | null) {
    this.spotlightVideoId = null;

    if (!!authInfo) {
      if (authInfo.sub != this.channel.id) {
        const channelData = this.route.parent?.snapshot.data[
          'channelData'
        ] as ChannelData;
        if (channelData.subscriptionStatus?.isSubscribed) {
          this.spotlightVideoId = this.channel.subscribedSpotlightVideoId;
        } else {
          this.spotlightVideoId = this.channel.unsubscribedSpotlightVideoId;
        }
      }
    } else {
      this.spotlightVideoId = this.channel.unsubscribedSpotlightVideoId;
    }
  }

  get pending$() {
    if (this.contextId == null) {
      return of(true);
    }

    return this.store.select(
      selectIsChannelSectionsLoading(this.channel.id, this.contextId)
    );
  }

  get sections$() {
    return this.store.select(
      selectChannelSectionInstances(
        this.channel.id,
        this.channel.sections.map((x) => x.id).filter((x): x is string => !!x)
      )
    );
  }
}
