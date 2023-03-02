import {concatLatestFrom} from '@ngrx/effects';
import {filter, map, take, tap} from 'rxjs';
import {selectVideoForumState} from './../selectors/index';
import {VideoCommentClient} from './models/index';
import {selectCommentsCount, selectHasMoreComments} from './selectors/index';
import {Store} from '@ngrx/store';
import {Component, ElementRef, Input, OnChanges, OnDestroy, OnInit, SimpleChanges, ViewChild,} from '@angular/core';
import {VideoCommentSort} from './models';
import {VideoForumAction} from './actions';
import {selectRootComments} from './selectors';
import {AuthService} from 'src/app/auth/services/auth.service';
import {UserProfileService} from 'src/app/core/services/user-profile.service';

@Component({
  selector: 'app-video-forum',
  templateUrl: './video-forum.component.html',
  styleUrls: ['./video-forum.component.css'],
})
export class VideoForumComponent implements OnInit, OnChanges, OnDestroy {
  @Input()
  videoId!: string;

  @Input()
  rootPageSize: number = 20;

  @Input()
  commentPageSize: number = 10;

  @Input()
  sort: VideoCommentSort = VideoCommentSort.LikesCount;

  @Input()
  showLoadMoreRootCommentsButton: boolean = true;

  @ViewChild('bottomSection', { static: true })
  bottomSection!: ElementRef;

  private bottomSectionObserver: IntersectionObserver;

  constructor(
    private store: Store,
    private authService: AuthService,
    private userProfileService: UserProfileService
  ) {
    this.bottomSectionObserver = new IntersectionObserver(
      this.onObserveBottom.bind(this),
      {
        root: null,
        threshold: 0,
      }
    );
  }

  ngOnDestroy(): void {
    this.bottomSectionObserver.unobserve(this.bottomSection.nativeElement);
    this.bottomSectionObserver.disconnect();
  }

  ngOnInit(): void {
    this.bottomSectionObserver.observe(this.bottomSection.nativeElement);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['videoId']) {
      this.sortByTopComments();
    }
  }

  onObserveBottom(
    entries: IntersectionObserverEntry[],
    observer: IntersectionObserver
  ) {
    const [entry] = entries;

    if (entry.isIntersecting && !this.showLoadMoreRootCommentsButton) {
      this.store
        .select(selectVideoForumState)
        .pipe(
          take(1),
          filter((x) => x.loaded && !x.pending && !x.loadingMoreComments),
          concatLatestFrom(() => this.hasMoreComments$),
          tap(([_, hasMoreComments]) => {
            if (hasMoreComments) {
              this.showMoreComments();
            }
          })
        )
        .subscribe();
    }
  }

  get isAuthenticated$() {
    return this.authService.isAuthenticated$;
  }

  get isUserReady$() {
    return this.userProfileService.userReady$;
  }

  get rootComments$() {
    return this.store.select(selectRootComments);
  }

  get commentsCount$() {
    return this.store.select(selectCommentsCount);
  }

  get initializing$() {
    return this.store
      .select(selectVideoForumState)
      .pipe(map((x) => x.pending && !x.loaded));
  }

  get loading$() {
    return this.store
      .select(selectVideoForumState)
      .pipe(map((x) => x.pending && x.loaded));
  }

  get loadingMoreComments$() {
    return this.store
      .select(selectVideoForumState)
      .pipe(map((x) => x.loadingMoreComments));
  }

  showMoreComments() {
    this.store.dispatch(VideoForumAction.getRootComments());
  }

  trackCommentClient(
    index: number,
    videoCommentClient: VideoCommentClient
  ): any {
    return videoCommentClient.id;
  }

  get hasMoreComments$() {
    return this.store.select(selectHasMoreComments);
  }

  sortByTopComments() {
    this.store.dispatch(
      VideoForumAction.loadForum({
        videoId: this.videoId,
        rootPageSize: this.rootPageSize,
        commentPageSize: this.commentPageSize,
        sort: VideoCommentSort.LikesCount,
      })
    );
  }

  sortByNewestFirst() {
    this.store.dispatch(
      VideoForumAction.loadForum({
        videoId: this.videoId,
        rootPageSize: this.rootPageSize,
        commentPageSize: this.commentPageSize,
        sort: VideoCommentSort.Date,
      })
    );
  }
}
