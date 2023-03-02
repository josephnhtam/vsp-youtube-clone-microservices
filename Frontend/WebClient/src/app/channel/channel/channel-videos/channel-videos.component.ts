import {selectChannelPage, selectHasMoreChannelPageItems,} from '../../../core/selectors/channel-page';
import {Store} from '@ngrx/store';
import {Component, ElementRef, OnDestroy, OnInit, ViewChild,} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {filter, map, Subscription, take, tap} from 'rxjs';
import {ChannelData, Video,} from 'src/app/core/models/channel';
import {ChannelPageAction} from '../../../core/actions';
import {ChannelPageType, VideosPageContent,} from '../../../core/reducers/channel-page';
import {concatLatestFrom} from '@ngrx/effects';

@Component({
  selector: 'app-channel-videos',
  templateUrl: './channel-videos.component.html',
  styleUrls: ['./channel-videos.component.css'],
})
export class ChannelVideosComponent implements OnInit, OnDestroy {
  channel!: ChannelData;

  @ViewChild('bottomSection', { static: true })
  bottomSection!: ElementRef;

  private bottomSectionObserver: IntersectionObserver;
  private channelSub?: Subscription;
  private pageSize = 28;

  constructor(private route: ActivatedRoute, private store: Store) {
    this.bottomSectionObserver = new IntersectionObserver(
      this.onObserveBottom.bind(this),
      {
        root: null,
        threshold: 0,
      }
    );

    this.channelSub = this.route
      .parent!.data.pipe(map((data) => data['channelData'] as ChannelData))
      .subscribe((channel) => {
        this.channel = channel;
        this.onChannelRefreshed();
      });
  }

  ngOnDestroy(): void {
    this.channelSub?.unsubscribe();
  }

  ngOnInit(): void {
    this.bottomSectionObserver.observe(this.bottomSection.nativeElement);
  }

  onChannelRefreshed() {
    this.channelPage$
      .pipe(
        take(1),

        filter((page) => {
          return !page?.loaded;
        }),

        tap(() => {
          this.store.dispatch(
            ChannelPageAction.loadChannelPage({
              userId: this.channel.userProfile.id,
              contextId: 'videos',
              pageSize: this.pageSize,
              request: {
                type: ChannelPageType.UploadedVideos,
              },
            })
          );
        })
      )
      .subscribe();
  }

  onObserveBottom(
    entries: IntersectionObserverEntry[],
    observer: IntersectionObserver
  ) {
    const [entry] = entries;

    if (entry.isIntersecting) {
      this.hasMoreItems$
        .pipe(
          take(1),

          concatLatestFrom(() => this.loaded$),

          tap(([hasMoreItems, loaded]) => {
            if (hasMoreItems && loaded) {
              this.showMoreResults();
            }
          })
        )
        .subscribe();
    }
  }

  showMoreResults() {
    this.store.dispatch(
      ChannelPageAction.loadMoreResults({
        userId: this.channel.userProfile.id,
        contextId: 'videos',
      })
    );
  }

  get channelPage$() {
    return this.store.select(
      selectChannelPage(this.channel.userProfile.id, 'videos')
    );
  }

  get channelPageConten$() {
    return this.channelPage$.pipe(
      map((page) => page?.content as VideosPageContent)
    );
  }

  get videos$() {
    return this.channelPageConten$.pipe(
      map((content) => content?.items as Video[])
    );
  }

  get loaded$() {
    return this.channelPage$.pipe(map((page) => page?.loaded));
  }

  get pending$() {
    return this.channelPage$.pipe(map((page) => page?.pending));
  }

  get hasMoreItems$() {
    return this.store.select(
      selectHasMoreChannelPageItems(this.channel.userProfile.id, 'videos')
    );
  }
}
