import {concatLatestFrom} from '@ngrx/effects';
import {AuthService} from 'src/app/auth/services/auth.service';
import {ChannelData, ChannelSectionType, SpotlightVideoSection,} from '../../../core/models/channel';
import {catchError, map} from 'rxjs';
import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, Resolve, Router, RouterStateSnapshot,} from '@angular/router';
import {UserChannel} from 'src/app/core/models/channel';
import {ChannelHelperService} from 'src/app/core/services/channel-helper.service';
import {AuthInfo} from 'src/app/auth/models/auth-info';

@Injectable()
export class ChannelFeaturedResolver implements Resolve<UserChannel> {
  private maxSectionItemsCount = 12;

  constructor(
    private router: Router,
    private service: ChannelHelperService,
    private authService: AuthService
  ) {}

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    const data = route.parent?.data['channelData'];
    const userId = data.userProfile.id;

    return this.service.getChannel(userId, this.maxSectionItemsCount).pipe(
      catchError((error) => {
        this.failed(error, route, state);
        throw error;
      }),

      concatLatestFrom(() => this.authService.authInfo$),

      map(([channel, authInfo]) => {
        const spotlightVideoId = this.getSpotlightVideoId(
          channel,
          data,
          authInfo
        );

        if (!spotlightVideoId) {
          return channel;
        }

        const spotlightVideoSection: SpotlightVideoSection = {
          type: ChannelSectionType.SpotlightVideo,
          id: 'spotlight',
          content: {
            videoId: spotlightVideoId,
          },
        };

        channel = {
          ...channel,
          sections: [spotlightVideoSection, ...channel.sections],
        };

        return channel;
      })
    );
  }

  private getSpotlightVideoId(
    channel: UserChannel,
    channelData: ChannelData,
    authInfo: AuthInfo | null
  ) {
    let spotlightVideoId: string | null = null;

    if (!!authInfo) {
      if (authInfo.sub != channel.id) {
        if (channelData.subscriptionStatus?.isSubscribed) {
          spotlightVideoId = channel.subscribedSpotlightVideoId;
        } else {
          spotlightVideoId = channel.unsubscribedSpotlightVideoId;
        }
      }
    } else {
      spotlightVideoId = channel.unsubscribedSpotlightVideoId;
    }

    return spotlightVideoId;
  }

  private failed(
    error: any,
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ) {
    console.error(error);
    this.router.navigate(['/']);
  }
}
