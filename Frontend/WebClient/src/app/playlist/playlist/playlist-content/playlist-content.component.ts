import {AuthService} from '../../../auth/services/auth.service';
import {selectHasMoreVideos, selectLoadingMoreVideos, selectPlaylistItems,} from '../../../core/selectors/playlist';
import {filter, map, of, Subscription, take, tap} from 'rxjs';
import {Store} from '@ngrx/store';
import {PlaylistItem} from 'src/app/core/models/library';
import {CdkDragDrop, moveItemInArray} from '@angular/cdk/drag-drop';
import {Component, ElementRef, Input, OnDestroy, OnInit, ViewChild,} from '@angular/core';
import {PlaylistAction} from '../../../core/actions';
import {concatLatestFrom} from '@ngrx/effects';
import {selectPlaylistState} from "../../../core/selectors";

@Component({
  selector: 'app-playlist-content',
  templateUrl: './playlist-content.component.html',
  styleUrls: ['./playlist-content.component.css'],
})
export class PlaylistContentComponent implements OnInit, OnDestroy {
  @Input()
  playlistId!: string;

  @Input()
  draggable = true;

  items: PlaylistItem[] = [];

  private itemsSub?: Subscription;
  private isDragging = false;

  @ViewChild('bottomSection', { static: true })
  bottomSection!: ElementRef;

  private bottomSectionObserver: IntersectionObserver;

  constructor(private store: Store, private authService: AuthService) {
    this.bottomSectionObserver = new IntersectionObserver(
      this.onObserveBottom.bind(this),
      {
        root: null,
        threshold: 0,
      }
    );
  }

  ngOnInit(): void {
    this.itemsSub = this.store
      .select(selectPlaylistItems)
      .pipe(
        tap((items) => {
          this.items = [...items];
        })
      )
      .subscribe();

    this.bottomSectionObserver.observe(this.bottomSection.nativeElement);
  }

  ngOnDestroy(): void {
    this.itemsSub?.unsubscribe();
    this.bottomSectionObserver.unobserve(this.bottomSection.nativeElement);
    this.bottomSectionObserver.disconnect();
  }

  drop(event: CdkDragDrop<PlaylistItem[]>) {
    const item = this.items[event.previousIndex];

    const precedingIndex =
      event.previousIndex < event.currentIndex
        ? event.currentIndex
        : event.currentIndex - 1;

    const precedingItem =
      precedingIndex >= 0 ? this.items[precedingIndex] : null;

    moveItemInArray(this.items, event.previousIndex, event.currentIndex);

    this.store.dispatch(
      PlaylistAction.movePlaylistItem({
        itemId: item.id,
        precedingItemId: precedingItem?.id ?? null,
      })
    );
  }

  get isDraggable$() {
    return this.isReadonly$.pipe(
      map((isReadonly) => !isReadonly && this.draggable)
    );
  }

  get isVideosList() {
    return this.playlistId.toUpperCase().startsWith('VIDEOS-');
  }

  get isReadonly$() {
    if (this.isVideosList) {
      return of(true);
    }

    return this.store.select(selectPlaylistState).pipe(
      concatLatestFrom(() => this.authService.authInfo$),
      map(([state, authInfo]) => {
        return state.playlistInfo?.creatorProfile?.id != authInfo?.sub;
      })
    );
  }

  get loadingMoreVideos$() {
    return this.store.select(selectLoadingMoreVideos);
  }

  get hasMoreVideos$() {
    return this.store.select(selectHasMoreVideos);
  }

  showMoreVideos() {
    this.store.dispatch(PlaylistAction.loadMoreVideos());
  }

  dragStarted() {
    this.isDragging = true;
  }

  dragReleased() {
    this.isDragging = false;
  }

  onObserveBottom(
    entries: IntersectionObserverEntry[],
    observer: IntersectionObserver
  ) {
    const [entry] = entries;

    if (entry.isIntersecting) {
      this.store
        .select(selectPlaylistState)
        .pipe(
          filter((x) => !this.isDragging),
          take(1),
          filter((x) => x.loaded && !x.pending && !x.loadingMoreVideos),
          concatLatestFrom(() => this.hasMoreVideos$),
          tap(([_, hasMoreCommentss]) => {
            if (hasMoreCommentss) {
              this.showMoreVideos();
            }
          })
        )
        .subscribe();
    }
  }
}
