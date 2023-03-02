import {Component, ElementRef, Input, OnChanges, OnInit, SimpleChanges, ViewChild,} from '@angular/core';
import {concatLatestFrom} from '@ngrx/effects';
import {Store} from '@ngrx/store';
import {filter, map, Subscription, take, tap} from 'rxjs';
import {SearchAction} from '../../../core/actions';
import {isVideo, SearchableItem, Video as SearchableVideo} from '../../../core/models/search';
import {Video} from '../../../core/models/video';
import {selectHasMoreSearchResults, selectSearchResults,} from '../../../core/selectors/search';
import {selectSearchState} from "../../../core/selectors";

@Component({
  selector: 'app-relevant-videos',
  templateUrl: './relevant-videos.component.html',
  styleUrls: ['./relevant-videos.component.css'],
})
export class RelevantVideosComponent implements OnInit, OnChanges {
  tags?: string[];

  pageSize: number = 30;

  @Input()
  video!: Video;

  @Input()
  autoLoadMore = true;

  @ViewChild('bottomSection', { static: true })
  bottomSection!: ElementRef;

  private bottomSectionObserver: IntersectionObserver;
  private querySub?: Subscription;

  constructor(private store: Store) {
    this.bottomSectionObserver = new IntersectionObserver(
      this.onObserveBottom.bind(this),
      {
        root: null,
        threshold: 0,
      }
    );
  }

  ngOnChanges(changes: SimpleChanges): void {
    this.store.dispatch(
      SearchAction.searchRelevantVideos({
        pageSize: this.pageSize,
        video: this.video,
      })
    );
  }

  ngOnDestroy(): void {
    this.querySub?.unsubscribe();
    this.bottomSectionObserver.unobserve(this.bottomSection.nativeElement);
    this.bottomSectionObserver.disconnect();
  }

  ngOnInit(): void {
    this.bottomSectionObserver.observe(this.bottomSection.nativeElement);
  }

  get noResultsFound$() {
    return this.searchState$.pipe(
      map((x) => (x.loaded || x.error) && x.ids.length == 0)
    );
  }

  get searchState$() {
    return this.store.select(selectSearchState);
  }

  get searchResults$() {
    return this.store.select(selectSearchResults).pipe(
      map((results) => {
        return results.filter((x) => x.id !== this.video.id);
      })
    );
  }

  isItemVideo(item: SearchableItem): item is SearchableVideo {
    return isVideo(item);
  }

  asVideo(item: SearchableItem) {
    return item as SearchableVideo;
  }

  get isLoaded$() {
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
    if (!this.autoLoadMore) return;

    const [entry] = entries;

    if (entry.isIntersecting) {
      this.searchState$
        .pipe(
          take(1),
          filter((x) => x.loaded && !x.pending && !x.loadingMoreResults),
          concatLatestFrom(() => this.hasMoreResults$),
          tap(([_, hasMoreCommentss]) => {
            if (hasMoreCommentss) {
              this.loadMoreResults();
            }
          })
        )
        .subscribe();
    }
  }
}
