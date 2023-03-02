import {Actions, ofType} from '@ngrx/effects';
import {loadPlaylist} from '../../core/actions/playlist';
import {Store} from '@ngrx/store';
import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, Resolve, Router, RouterStateSnapshot,} from '@angular/router';
import {map} from 'rxjs';
import {PlaylistApiAction} from '../../core/actions';

@Injectable()
export class PlaylistResolver implements Resolve<string | null> {
  private pageSize: number = 50;

  constructor(
    private store: Store,
    private router: Router,
    private actions$: Actions
  ) {}

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    const playlistParams = route.queryParams as PlaylistParams;
    const playlistId = playlistParams.list;
    this.store.dispatch(loadPlaylist({ playlistId, pageSize: this.pageSize }));

    return this.actions$.pipe(
      ofType(
        PlaylistApiAction.playlistLoaded,
        PlaylistApiAction.failedToLoadPlaylist
      ),

      map((action) => {
        if (action.type === PlaylistApiAction.playlistLoaded.type) {
          return playlistId;
        } else {
          this.router.navigate(['/', 'unavailable', 'playlist'], {
            skipLocationChange: true,
          });
          return null;
        }
      })
    );
  }
}

export interface PlaylistParams {
  list: string;
}
