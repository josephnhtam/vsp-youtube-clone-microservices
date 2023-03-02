import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, Resolve, Router, RouterStateSnapshot,} from '@angular/router';
import {catchError, map} from 'rxjs';
import {ChannelHelperService} from 'src/app/core/services/channel-helper.service';
import {ChannelSectionType, UserChannel} from 'src/app/core/models/channel';

@Injectable()
export class ChannelPlaylistsResolver implements Resolve<UserChannel> {
  private maxSectionItemsCount = 12;

  constructor(private router: Router, private service: ChannelHelperService) {}

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    const data = route.parent?.data['channelData'];
    const userId = data.userProfile.id;

    return this.service
      .getChannel(
        userId,
        this.maxSectionItemsCount,
        ChannelSectionType.CreatedPlaylists |
          ChannelSectionType.MultiplePLaylists
      )
      .pipe(
        map((channel) => {
          let createdPlaylistsSection = channel.sections.find(
            (x) => x.type == ChannelSectionType.CreatedPlaylists
          );

          if (!createdPlaylistsSection) {
            createdPlaylistsSection = {
              type: ChannelSectionType.CreatedPlaylists,
              id: 'created-playlists',
            };
          }

          channel.sections = [
            createdPlaylistsSection,
            ...channel.sections.filter(
              (x) => x.type === ChannelSectionType.MultiplePLaylists
            ),
          ];

          return channel;
        }),

        catchError((error) => {
          this.failed(error, route, state);
          throw error;
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
