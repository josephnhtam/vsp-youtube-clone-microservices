import { AuthService } from 'src/app/auth/services/auth.service';
import { ActivatedRoute, Router } from '@angular/router';
import {
  Component,
  ElementRef,
  OnDestroy,
  OnInit,
  ViewChild,
} from '@angular/core';
import { map, Observable, Subscription, tap } from 'rxjs';
import { Actions, concatLatestFrom, ofType } from '@ngrx/effects';
import {
  PlaylistManagementAction,
  PlaylistManagementApiAction,
} from 'src/app/core/actions';
import { Store } from '@ngrx/store';
import { selectPlaylistState } from '../../core/selectors';

@Component({
  selector: 'app-playlist',
  templateUrl: './playlist.component.html',
  styleUrls: ['./playlist.component.css'],
})
export class PlaylistComponent implements OnInit, OnDestroy {
  playlistId$!: Observable<string>;

  private playlistRemovedSub: Subscription;
  private stateSub: Subscription;
  private dataSub: Subscription;

  @ViewChild('container') containerEl!: ElementRef;
  @ViewChild('content') contentEl!: ElementRef;

  constructor(
    private store: Store,
    private route: ActivatedRoute,
    private actions$: Actions,
    private router: Router,
    private authService: AuthService
  ) {
    this.playlistId$ = this.route.data.pipe(map((x) => x['playlistId']));

    this.dataSub = this.route.data.subscribe(this.resetScrollPos.bind(this));

    this.playlistRemovedSub = this.actions$
      .pipe(
        ofType(PlaylistManagementApiAction.playlistRemoved),
        concatLatestFrom(() => this.playlistId$),
        tap(([{ playlistId }, currentPlaylistId]) => {
          if (playlistId == currentPlaylistId) {
            this.router.navigate(['/'], {
              replaceUrl: true,
            });
          }
        })
      )
      .subscribe();

    this.stateSub = this.state$
      .pipe(concatLatestFrom(() => this.authService.authInfo$))
      .subscribe(([state, authInfo]) => {
        if (
          !state.loaded ||
          !state.id ||
          !state.playlistInfo ||
          state.playlistInfo.creatorProfile.id === authInfo?.sub
        ) {
          return;
        }

        const info = state.playlistInfo;
        this.store.dispatch(
          PlaylistManagementAction.checkPlaylistRef({
            playlistId: state.id!,
            title: info.title ?? undefined,
          })
        );
      });
  }

  ngOnDestroy(): void {
    this.playlistRemovedSub.unsubscribe();
    this.stateSub.unsubscribe();
    this.dataSub.unsubscribe();
  }

  ngOnInit(): void {}

  resetScrollPos() {
    if (this.containerEl) {
      this.containerEl.nativeElement.scrollTop = 0;
    }

    if (this.contentEl) {
      this.contentEl.nativeElement.scrollTop = 0;
    }
  }

  get draggable$() {
    return this.playlistId$.pipe(
      map((playlistId) => {
        const id = playlistId.toUpperCase();
        return id != 'LL' && id != 'DL' && !id.startsWith('VIDEOS-');
      })
    );
  }

  get state$() {
    return this.store.select(selectPlaylistState);
  }
}
