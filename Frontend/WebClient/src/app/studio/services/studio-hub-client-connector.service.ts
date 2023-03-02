import {concatMap, EMPTY, take} from 'rxjs';
import {StudioHubClientService} from './studio-hub-client.service';
import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, CanDeactivate, RouterStateSnapshot,} from '@angular/router';
import {StudioComponent} from '../studio.component';

@Injectable()
export class StudioHubClientConnectorService
  implements CanActivate, CanDeactivate<StudioComponent>
{
  constructor(private studioHubClient: StudioHubClientService) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    this.studioHubClient.isStarted$
      .pipe(
        take(1),
        concatMap((isStarted) => {
          if (!isStarted) {
            return this.studioHubClient.start();
          }
          return EMPTY;
        })
      )
      .subscribe();

    return true;
  }

  canDeactivate(
    component: StudioComponent,
    currentRoute: ActivatedRouteSnapshot,
    currentState: RouterStateSnapshot,
    nextState?: RouterStateSnapshot | undefined
  ) {
    this.studioHubClient.isStarted$
      .pipe(
        take(1),
        concatMap((isStarted) => {
          if (isStarted) {
            return this.studioHubClient.stop();
          }
          return EMPTY;
        })
      )
      .subscribe();

    return true;
  }
}
