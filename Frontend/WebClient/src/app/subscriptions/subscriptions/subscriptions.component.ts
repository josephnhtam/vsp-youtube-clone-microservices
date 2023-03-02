import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import {
  Component,
  ElementRef,
  OnDestroy,
  OnInit,
  ViewChild,
} from '@angular/core';
import { concatLatestFrom } from '@ngrx/effects';
import { Store } from '@ngrx/store';
import { filter, map, of, Subscription, take, tap } from 'rxjs';
import { SearchAction } from '../../core/actions';
import {
  SearchByCreatorsRequest,
  SearchSort,
  Video,
} from '../../core/models/search';
import {
  selectHasMoreSearchResults,
  selectSearchResults,
} from '../../core/selectors/search';
import { SubscriptionTargetIds } from '../../core/models/subscription';
import { selectSearchState } from '../../core/selectors';

@Component({
  selector: 'app-subscriptions',
  templateUrl: './subscriptions.component.html',
  styleUrls: ['./subscriptions.component.css'],
})
export class SubscriptionsComponent implements OnInit, OnDestroy {
  @ViewChild('bottomSection', { static: true })
  bottomSection!: ElementRef;

  pageSize: number = 48;
  videosGroups: VideoGroup[] = [];

  private videoSet: Set<string> = new Set<string>();
  private subscriptionTargetIds: string[] | null = null;
  private bottomSectionObserver: IntersectionObserver;

  private searchResultsSub?: Subscription;

  constructor(private store: Store, private httpClient: HttpClient) {
    this.bottomSectionObserver = new IntersectionObserver(
      this.onObserveBottom.bind(this),
      {
        root: null,
        threshold: 0,
      }
    );

    this.searchSubscriptions();
  }

  ngOnDestroy(): void {
    this.searchResultsSub?.unsubscribe();
    this.bottomSectionObserver.unobserve(this.bottomSection.nativeElement);
    this.bottomSectionObserver.disconnect();
  }

  ngOnInit(): void {
    this.bottomSectionObserver.observe(this.bottomSection.nativeElement);
  }

  createSearchResultsSubscription() {
    this.searchResultsSub = this.searchResults$.subscribe((videos) => {
      let videoGroup: VideoGroup = new VideoGroup(TimePeriod.today);

      for (const video of videos) {
        if (!this.videoSet.has(video.id)) {
          this.videoSet.add(video.id);

          const currentPeriod = getTimePeriod(video.createDate);

          if (currentPeriod != videoGroup.period) {
            if (videoGroup.videos.length > 0) {
              this.videosGroups.push(videoGroup);
            }

            videoGroup = new VideoGroup(currentPeriod);
          }

          videoGroup.videos.push(video);
        }
      }

      if (videoGroup.videos.length > 0) {
        this.videosGroups.push(videoGroup);
      }
    });
  }

  searchSubscriptions() {
    this.store.dispatch(SearchAction.reset());

    this.createSearchResultsSubscription();

    if (this.subscriptionTargetIds == null) {
      const url = environment.appSetup.apiUrl + '/api/v1/Subscriptions/Ids';

      this.httpClient.get<SubscriptionTargetIds>(url).subscribe({
        next: (response) => {
          this.subscriptionTargetIds = response.subscriptionTargetIds;

          if (this.subscriptionTargetIds.length > 0) {
            const searchRequest: SearchByCreatorsRequest = {
              type: 'creators',
              creatorIds: this.subscriptionTargetIds,
              sort: SearchSort.CreateDate,
              period: null,
            };

            this.store.dispatch(
              SearchAction.search({
                searchRequest,
                pageSize: this.pageSize,
              })
            );
          }
        },

        error: (error) => {
          console.error(error);
        },
      });
    }
  }

  get noResultsFound$() {
    if (
      this.subscriptionTargetIds != null &&
      this.subscriptionTargetIds.length === 0
    )
      return of(true);

    return this.searchState$.pipe(
      map((x) => (x.loaded || x.error) && x.ids.length == 0)
    );
  }

  get searchState$() {
    return this.store.select(selectSearchState);
  }

  get searchResults$() {
    return this.store
      .select(selectSearchResults)
      .pipe(map((results) => results.map((item) => item as Video)));
  }

  get isLoaded$() {
    if (this.subscriptionTargetIds == null) return of(false);
    if (this.subscriptionTargetIds.length === 0) return of(true);

    return this.searchState$.pipe(map((x) => x.loaded));
  }

  get tags$() {
    return this.searchState$.pipe(map((x) => x.tags));
  }

  get isLoadingMoreResults$() {
    return this.searchState$.pipe(map((x) => x.loadingMoreResults));
  }

  get hasMoreResults$() {
    return this.store.select(selectHasMoreSearchResults);
  }

  loadMoreResults() {
    this.store.dispatch(SearchAction.loadMoreResults());
  }

  onObserveBottom(
    entries: IntersectionObserverEntry[],
    observer: IntersectionObserver
  ) {
    const [entry] = entries;

    if (entry.isIntersecting) {
      this.searchState$
        .pipe(
          take(1),
          filter((x) => x.loaded && !x.pending && !x.loadingMoreResults),
          concatLatestFrom(() => this.hasMoreResults$),
          tap(([_, hasMoreResults]) => {
            if (hasMoreResults) {
              this.loadMoreResults();
            }
          })
        )
        .subscribe();
    }
  }
}

export class VideoGroup {
  videos: Video[] = [];
  constructor(public period: TimePeriod) {}
}

let today = new Date();
today.setHours(0, 0, 0, 0);

let yesterday = new Date();
yesterday.setDate(yesterday.getDate() - 1);
yesterday.setHours(0, 0, 0, 0);

let week = new Date();
week.setDate(week.getDate() - 7);
week.setHours(0, 0, 0, 0);

let month = new Date();
month.setMonth(month.getMonth() - 1);
month.setHours(0, 0, 0, 0);

export enum TimePeriod {
  today,
  yesterday,
  week,
  month,
  older,
}

function getTimePeriod(createDate: string) {
  let date = new Date(createDate);

  if (date > today) {
    return TimePeriod.today;
  } else if (date > yesterday) {
    return TimePeriod.yesterday;
  } else if (date > week) {
    return TimePeriod.week;
  } else if (date > month) {
    return TimePeriod.month;
  } else {
    return TimePeriod.older;
  }
}
