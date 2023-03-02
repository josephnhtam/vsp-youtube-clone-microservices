import { AuthInfo } from 'src/app/auth/models/auth-info';
import { checkPlaylistRef } from '../../../core/actions/playlist-management';
import {
  selectPlaylistManagementState,
  selectVideoPlaylistState,
} from '../../../core/selectors';
import { AuthService } from 'src/app/auth/services/auth.service';
import { concatLatestFrom } from '@ngrx/effects';
import { ActivatedRoute, Params, Router } from '@angular/router';
import {
  Component,
  Input,
  OnChanges,
  OnDestroy,
  OnInit,
  SimpleChanges,
} from '@angular/core';
import {
  EMPTY,
  map,
  Observable,
  of,
  Subscription,
  switchMap,
  take,
  tap,
} from 'rxjs';
import { Store } from '@ngrx/store';
import { VideoPlaylistAction } from '../../../core/actions';
import { PlaylistManagementAction } from 'src/app/core/actions';
import { PlaylistVisibility } from '../../../core/models/library';
import {
  selectAvailableVideoPlaylistItems,
  selectVideoPlaylistItems,
} from 'src/app/core/selectors/video-playlist';
import { PlaylistInfo } from 'src/app/core/reducers/video-playlist';

@Component({
  selector: 'app-video-playlist',
  templateUrl: './video-playlist.component.html',
  styleUrls: ['./video-playlist.component.css'],
})
export class VideoPlaylistComponent implements OnInit, OnChanges, OnDestroy {
  @Input()
  videoId!: string;

  @Input()
  pageSize: number = 200;

  @Input()
  loopState: PlaylistLoopState = PlaylistLoopState.None;

  @Input()
  shuffle: boolean = false;

  private playlistId: string | null = null;
  private index: number | null = null;

  private playedIndices: number[] = [];
  private playedIndicesSet: Set<number> = new Set<number>();

  private queryParamsSub: Subscription;
  private playlistInfoSub: Subscription;

  private lastPlaylistId?: string;

  constructor(
    private store: Store,
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService
  ) {
    this.queryParamsSub = this.route.queryParams.subscribe(
      this.onQueryParams.bind(this)
    );

    this.playlistInfoSub = this.playlistInfo$
      .pipe(concatLatestFrom(() => this.authService.authInfo$))
      .subscribe(([info, authInfo]) => {
        this.checkPlaylistRef(info, authInfo);
      });
  }

  ngOnDestroy(): void {
    this.queryParamsSub.unsubscribe();
    this.playlistInfoSub.unsubscribe();
  }

  ngOnInit(): void {
    this.store.dispatch(VideoPlaylistAction.reset());
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['videoId'] != null) {
      this.loadPlaylist();
    }
  }

  onQueryParams(params: Params) {
    this.playlistId = params['list'];

    const indexParam = params['index'];
    if (indexParam != null && !Number.isNaN(indexParam)) {
      this.index = +indexParam;
    } else {
      this.index = null;
    }
  }

  checkPlaylistRef(info: PlaylistInfo | null, authInfo: AuthInfo | null) {
    if (!info || !authInfo) return;

    if (
      info.creatorProfile.id == authInfo.sub ||
      info.id == this.lastPlaylistId
    ) {
      return;
    }

    const playlistIdUpper = info.id.toUpperCase();
    if (
      playlistIdUpper == 'LL' ||
      playlistIdUpper == 'WL' ||
      playlistIdUpper == 'DL'
    ) {
      return;
    }

    this.lastPlaylistId = info.id;

    this.store.dispatch(
      PlaylistManagementAction.checkPlaylistRef({
        playlistId: info.id,
        title: info.title ?? undefined,
      })
    );
  }

  get isMine$() {
    return this.playlistInfo$.pipe(
      concatLatestFrom(() => this.authService.authInfo$),
      map(([playlistInfo, authInfo]) => {
        return playlistInfo?.creatorProfile.id === authInfo?.sub;
      })
    );
  }

  get isAuthenticated$() {
    return this.authService.isAuthenticated$;
  }

  get hasPlaylistRef$(): Observable<boolean> {
    return this.store
      .select(selectPlaylistManagementState)
      .pipe(
        map((x) => !!this.playlistId && x.entities[this.playlistId] != null)
      );
  }

  get state$() {
    return this.store.select(selectVideoPlaylistState);
  }

  get failedToLoad$() {
    return this.state$.pipe(
      map((state) => {
        return (
          state.id == this.playlistId && !state.loaded && state.error != null
        );
      })
    );
  }

  get playlistInfo$() {
    return this.state$.pipe(map((x) => x.playlistInfo));
  }

  get playlistItems$() {
    return this.store.select(selectVideoPlaylistItems);
  }

  get availablePlaylistItems$() {
    return this.store.select(selectAvailableVideoPlaylistItems);
  }

  get isLoaded$() {
    return this.state$.pipe(map((x) => x.loaded));
  }

  get currentItem$() {
    return this.playlistItems$.pipe(
      map((items) => {
        if (this.index != null) {
          const item = items.find((item) => item.position === this.index);
          if (item?.video.id === this.videoId) {
            return item;
          }
        }

        return items.find((item) => item.video.id === this.videoId);
      })
    );
  }

  get visibilityIcon$() {
    return this.playlistInfo$.pipe(
      map((info) => {
        switch (info!.visibility) {
          default:
          case PlaylistVisibility.Private:
            return 'lock';
          case PlaylistVisibility.Unlisted:
            return 'link';
          case PlaylistVisibility.Public:
            return 'public';
        }
      })
    );
  }

  get visibility$() {
    return this.playlistInfo$.pipe(
      map((info) => {
        const visibility = info!.visibility;

        switch (visibility ?? PlaylistVisibility.Private) {
          case PlaylistVisibility.Private:
            return 'Private';
          case PlaylistVisibility.Unlisted:
            return 'Unlisted';
          case PlaylistVisibility.Public:
            return 'Public';
        }
      })
    );
  }

  get creatorDisplayName$() {
    return this.playlistInfo$.pipe(
      map((info) => info!.creatorProfile.displayName)
    );
  }

  get currentPosition$() {
    return this.currentItem$.pipe(
      map((item) => {
        if (item != null) {
          return (item.position ?? 0) + 1;
        }
        return 1;
      })
    );
  }

  get itemsCount$() {
    return this.playlistInfo$.pipe(map((info) => info?.itemsCount));
  }

  get hasOptions() {
    return !this.isVideosList;
  }

  get isVideosList() {
    return this.playlistId?.toUpperCase().startsWith('VIDEOS-');
  }

  get title$(): Observable<string | undefined> {
    const playlistId = this.playlistId?.toUpperCase();

    switch (playlistId) {
      case 'LL':
        return of('Liked videos');
      case 'DL':
        return of('Disliked videos');
      case 'WL':
        return of('Watch later');
    }

    if (this.isVideosList) {
      return of('Videos');
    }

    return this.playlistInfo$.pipe(map((x) => x?.title ?? undefined));
  }

  switchLoopState() {
    this.loopState++;
    if (this.loopState > PlaylistLoopState.SingleLoop) {
      this.loopState = PlaylistLoopState.None;
    }
  }

  toggleShuffle() {
    this.shuffle = !this.shuffle;
  }

  getPlaylistItem(index: number) {
    return this.playlistItems$.pipe(
      map((items) => {
        if (index >= 0 && index < items.length) {
          return items[index];
        } else {
          return null;
        }
      })
    );
  }

  loadPlaylist() {
    if (this.playlistId == null || this.videoId == null) {
      return;
    }

    this.store.dispatch(
      VideoPlaylistAction.loadPlaylist({
        playlistId: this.playlistId!,
        index: this.index,
        videoId: this.videoId,
        pageSize: this.pageSize,
      })
    );
  }

  viewPlaylist() {
    this.router.navigate(['/playlist'], {
      queryParams: {
        list: this.playlistId,
      },
    });
  }

  navigateToItem(index: number) {
    return this.getPlaylistItem(index).pipe(
      take(1),

      tap((item) => {
        if (item == null) return;

        this.router.navigate(['/watch', item.video.id], {
          queryParams: {
            list: this.playlistId,
            index,
          },
        });
      })
    );
  }

  navigateToPrevItem() {
    this.currentItem$
      .pipe(
        take(1),

        concatLatestFrom(() => this.availablePlaylistItems$),

        switchMap(([currentItem, availableItems]) => {
          if (availableItems.length === 0) return EMPTY;

          const currentIndex =
            currentItem != null ? currentItem.position : this.index;
          if (currentIndex == null) return EMPTY;

          availableItems = availableItems.reverse();

          let item = availableItems.find((x) => x.position! < currentIndex);

          if (item == null && this.loopState === PlaylistLoopState.Loop) {
            item = availableItems[availableItems.length - 1];
          }

          if (item == null) return EMPTY;
          return this.navigateToItem(item.position!);
        })
      )
      .subscribe();
  }

  navigateToNextItem() {
    this.currentItem$
      .pipe(
        take(1),

        concatLatestFrom(() => this.availablePlaylistItems$),

        switchMap(([currentItem, availableItems]) => {
          if (availableItems.length === 0) return EMPTY;

          const currentIndex =
            currentItem != null ? currentItem.position : this.index;
          if (currentIndex == null) return EMPTY;

          let item = availableItems.find((x) => x.position! > currentIndex);

          if (item == null && this.loopState === PlaylistLoopState.Loop) {
            item = availableItems[0];
          }

          if (item == null) return EMPTY;
          return this.navigateToItem(item.position!);
        })
      )
      .subscribe();
  }

  navigateToPrevItemShuffle() {
    const prevIndex = this.playedIndices.pop();
    if (prevIndex != undefined) {
      this.playedIndicesSet.delete(prevIndex);

      this.navigateToItem(prevIndex).subscribe();
    }
  }

  navigateToNextItemShuffle() {
    this.currentItem$
      .pipe(
        take(1),
        tap((item) => {
          if (item == null) return;

          if (!this.playedIndicesSet.has(item.position!)) {
            this.playedIndices.push(item.position!);
            this.playedIndicesSet.add(item.position!);
          }
        })
      )
      .subscribe();

    this.availablePlaylistItems$
      .pipe(
        take(1),

        switchMap((availableItems) => {
          if (availableItems.length == 0) return EMPTY;

          const getAvailableIndices = () => {
            const availableIndices: number[] = [];
            for (let item of availableItems) {
              const index = item.position!;
              if (!this.playedIndicesSet.has(index)) {
                availableIndices.push(index);
              }
            }
            return availableIndices;
          };

          let availableIndices = getAvailableIndices();

          if (
            availableIndices.length == 0 &&
            this.playedIndices.length > 0 &&
            this.loopState == PlaylistLoopState.Loop
          ) {
            this.playedIndices = [];
            this.playedIndicesSet.clear();
            this.navigateToNextItemShuffle();
            availableIndices = getAvailableIndices();
          }

          if (availableIndices.length > 0) {
            const nextIndex =
              availableIndices[
                Math.floor(Math.random() * availableIndices.length)
              ];
            this.playedIndices.push(nextIndex);
            this.playedIndicesSet.add(nextIndex);

            return this.navigateToItem(nextIndex);
          } else {
            return EMPTY;
          }
        })
      )
      .subscribe();
  }

  createPlaylistRef() {
    if (this.playlistId == null) return;

    this.title$.pipe(take(1)).subscribe((title) => {
      this.store.dispatch(
        PlaylistManagementAction.createPlaylistRef({
          playlistId: this.playlistId!,
          title: title,
        })
      );
    });
  }

  removePlaylistRef() {
    if (this.playlistId == null) return;

    this.title$.pipe(take(1)).subscribe((title) => {
      this.store.dispatch(
        PlaylistManagementAction.removePlaylistRef({
          playlistId: this.playlistId!,
        })
      );
    });
  }
}

export enum PlaylistLoopState {
  None,
  Loop,
  SingleLoop,
}
