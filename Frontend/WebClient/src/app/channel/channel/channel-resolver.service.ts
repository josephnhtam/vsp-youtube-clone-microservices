import {ChannelData} from '../../core/models/channel';
import {catchError, forkJoin, map, of} from 'rxjs';
import {Injectable} from '@angular/core';
import {ChannelHelperService} from 'src/app/core/services/channel-helper.service';
import {ActivatedRouteSnapshot, Resolve, Router, RouterStateSnapshot,} from '@angular/router';
import {ChannelSubscriptionService} from "../../core/services/channel-subscription.service";

@Injectable()
export class ChannelResolver implements Resolve<ChannelData | null> {
  constructor(
    private router: Router,
    private service: ChannelHelperService,
    private subscriptionService: ChannelSubscriptionService
  ) {}

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    return this.getUserProfileAndChannel(route, state);
  }

  getUserProfileAndChannel(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ) {
    const userId = route.params['userId'] as string | undefined;
    const handle = route.params['handle'] as string | undefined;

    if (!userId && !handle) {
      this.failed('no user id and handle provided', route, state);
      this.router.navigate(['/']);
      return of(null);
    } else {
      const request = {
        detailedUserChannelInfo: this.getDetailedUserChannelInfo(
          userId,
          handle
        ),

        subscriptionStatus: this.getSubscriptionStatus(userId, handle),
      };

      return forkJoin(request).pipe(
        map((result) => {
          if (result.detailedUserChannelInfo instanceof FailResponse) {
            const error = result.detailedUserChannelInfo.error;

            if (error.status == 400 || error.status == 404) {
              this.router.navigate(['/', 'unavailable', 'channel'], {
                skipLocationChange: true,
              });
            } else {
              this.router.navigate(['/'], {
                skipLocationChange: true,
              });
            }

            return null;
          }

          const channelData: ChannelData = {
            ...result.detailedUserChannelInfo!,
            subscriptionStatus: result.subscriptionStatus,
          };

          return channelData;
        })
      );
    }
  }

  private getSubscriptionStatus(userId?: string, handle?: string) {
    return this.subscriptionService.getSubscriptionStatus(userId, handle).pipe(
      catchError((error) => {
        return of(null);
      })
    );
  }

  private getDetailedUserChannelInfo(userId?: string, handle?: string) {
    return this.service.getChannelInfo(userId, handle).pipe(
      catchError((error) => {
        return of(new FailResponse(error));
      })
    );
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

class FailResponse {
  constructor(public error: any) {}
}
