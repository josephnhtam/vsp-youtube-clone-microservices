import {
  PlaylistLoopState,
  VideoPlaylistComponent,
} from './video-playlist/video-playlist.component';
import { VideoPlayerComponent } from '../../video-player/video-player/video-player.component';
import { VideoResource } from '../../video-player/video-player/models';
import { map, Observable, of, Subscription, take, tap } from 'rxjs';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { CommonToolbarService } from '../../shared/common-toolbar/common-toolbar.service';
import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ElementRef,
  HostListener,
  OnChanges,
  OnDestroy,
  OnInit,
  SimpleChanges,
  ViewChild,
} from '@angular/core';
import { Video, VideoStatus } from '../../core/models/video';
import * as moment from 'moment';
import { RecordUserWatchHistoryService } from './record-user-watch-history.service';

@Component({
  selector: 'app-watch-video',
  templateUrl: './watch-video.component.html',
  styleUrls: ['./watch-video.component.css'],
  providers: [RecordUserWatchHistoryService],
})
export class WatchVideoComponent
  implements OnInit, OnDestroy, OnChanges, AfterViewInit
{
  moreDesc = false;
  historyRecorded = false;

  video$!: Observable<Video>;
  videoResources$!: Observable<VideoResource[]>;
  thumbnailUrl$!: Observable<string | undefined>;
  videoChangedSub?: Subscription;

  playlistId: string | null = null;

  width: number = 1920;

  initialShuffle: boolean = false;

  @ViewChild('videoPlayer', { static: true })
  videoPlayer!: VideoPlayerComponent;

  @ViewChild('videoContainer', { static: true })
  videoContainerEl!: ElementRef;

  @ViewChild('playlistComp')
  playlistComp?: VideoPlaylistComponent;

  @ViewChild('container')
  containerEl!: ElementRef;

  afterViewInit = false;

  private dataSub?: Subscription;
  private queryParamsSub?: Subscription;

  constructor(
    private toolbarService: CommonToolbarService,
    private route: ActivatedRoute,
    private historyService: RecordUserWatchHistoryService,
    private router: Router,
    private cd: ChangeDetectorRef
  ) {
    this.toolbarService.startHideSidebar();

    const navigation = this.router.getCurrentNavigation();
    if (navigation?.extras.state?.['shuffle']) {
      this.initialShuffle = true;
    }
  }

  ngAfterViewInit(): void {
    setTimeout(() => {
      this.afterViewInit = true;
    }, 0);
  }

  ngOnChanges(changes: SimpleChanges): void {}

  get loop() {
    if (this.playlistComp != null) {
      return this.playlistComp.loopState === PlaylistLoopState.SingleLoop;
    }
    return false;
  }

  get hasPlaylist() {
    return this.playlistId != null;
  }

  get videoContainerHeight() {
    return this.videoContainerEl.nativeElement.getBoundingClientRect().height;
  }

  get playlistStyle() {
    return `max-height: ${this.videoContainerHeight}px`;
  }

  ngOnDestroy(): void {
    this.toolbarService.endHideSidebar();
    this.videoChangedSub?.unsubscribe();
    this.queryParamsSub?.unsubscribe();
    this.dataSub?.unsubscribe();
  }

  get isProcessing$() {
    return this.video$.pipe(map((x) => x.status == VideoStatus.Preparing));
  }

  get isVideoAvailable$() {
    return this.videoResources$.pipe(map((x) => x.length > 0));
  }

  get createDate$() {
    return this.video$.pipe(map((x) => moment(x.createDate).fromNow()));
  }

  get viewsCount$() {
    return this.video$.pipe(map((x) => x.metrics.viewsCount));
  }

  get playlistItemId$(): Observable<string | undefined> {
    if (!this.playlistComp) {
      return of(undefined);
    }

    return this.playlistComp.currentItem$.pipe(
      map((item) => item?.id ?? undefined)
    );
  }

  getVideoStyle$() {
    return this.video$.pipe(
      map((v) => {
        const aspectRatio = 16 / 9; //v.videos[0].width / v.videos[0].height;
        const bestRatio = 16 / 9;
        const ratio = Math.min(100, 100 * (aspectRatio / bestRatio));
        return `width: ${ratio}%; padding-top: ${(1 / bestRatio) * 100}%`;
      })
    );
  }

  toggleDesc() {
    this.moreDesc = !this.moreDesc;
    return false;
  }

  onVideoChanged(video: Video) {
    this.moreDesc = false;
    this.historyRecorded = false;
  }

  recordHistory() {
    if (this.historyRecorded) return;

    this.video$
      .pipe(
        take(1),
        tap((video) => {
          this.historyService.recordWatch(video.id).subscribe();
        })
      )
      .subscribe();

    this.historyRecorded = true;
  }

  onVideoPlayed() {
    this.recordHistory();
  }

  onVideoEnded() {
    if (this.playlistComp != null) {
      this.nextVideo();
    }
  }

  prevVideo() {
    if (this.playlistComp != null) {
      if (this.playlistComp.shuffle) {
        this.playlistComp.navigateToPrevItemShuffle();
      } else {
        this.playlistComp.navigateToPrevItem();
      }
    }
  }

  nextVideo() {
    if (this.playlistComp != null) {
      if (this.playlistComp.shuffle) {
        this.playlistComp.navigateToNextItemShuffle();
      } else {
        this.playlistComp.navigateToNextItem();
      }
    }
  }

  private onQueryParams(params: Params) {
    if (params['list'] != null) {
      this.playlistId = params['list'];
    } else {
      this.playlistId = null;
    }
  }

  ngOnInit(): void {
    this.dataSub = this.route.data.subscribe(this.onDataChanged.bind(this));

    this.video$ = this.route.data.pipe(map((data) => data['video'] as Video));

    this.videoResources$ = this.video$.pipe(map((video) => video.videos));

    this.thumbnailUrl$ = this.video$.pipe(
      map((video) => video.thumbnailUrl ?? undefined)
    );

    this.queryParamsSub = this.route.queryParams.subscribe(
      this.onQueryParams.bind(this)
    );

    this.videoChangedSub = this.video$.subscribe((video) => {
      this.onVideoChanged(video);
    });

    this.onResize();
  }

  onDataChanged() {
    if (this.containerEl?.nativeElement) {
      this.containerEl.nativeElement.scrollTop = 0;
    }
  }

  @HostListener('window:resize')
  onResize() {
    this.width = window.innerWidth;
  }
}
