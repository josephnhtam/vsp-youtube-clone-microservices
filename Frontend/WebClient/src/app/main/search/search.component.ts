import {ActivatedRoute, Params} from '@angular/router';
import {
  isVideo,
  SearchableItem,
  SearchByQueryRequest,
  SearchParams,
  SearchTarget,
  Video,
} from '../../core/models/search';
import {Component, ElementRef, OnDestroy, OnInit, ViewChild,} from '@angular/core';
import {concatLatestFrom} from '@ngrx/effects';
import {Store} from '@ngrx/store';
import {filter, map, Subscription, take, tap} from 'rxjs';
import {SearchAction} from '../../core/actions';
import {selectHasMoreSearchResults, selectSearchResults,} from '../../core/selectors/search';
import {selectSearchState} from "../../core/selectors";

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css'],
})
export class SearchComponent implements OnInit, OnDestroy {
  tags?: string[];

  pageSize: number = 30;

  @ViewChild('container', { static: true })
  containerEl!: ElementRef;

  @ViewChild('bottomSection', { static: true })
  bottomSection!: ElementRef;

  private bottomSectionObserver: IntersectionObserver;
  private querySub?: Subscription;

  constructor(private store: Store, private route: ActivatedRoute) {
    this.bottomSectionObserver = new IntersectionObserver(
      this.onObserveBottom.bind(this),
      {
        root: null,
        threshold: 0,
      }
    );

    route.queryParams.subscribe(this.onQuery.bind(this));
  }

  ngOnDestroy(): void {
    this.querySub?.unsubscribe();
    this.bottomSectionObserver.unobserve(this.bottomSection.nativeElement);
    this.bottomSectionObserver.disconnect();
  }

  ngOnInit(): void {
    this.bottomSectionObserver.observe(this.bottomSection.nativeElement);
  }

  onQuery(params: Params) {
    const searchParams: SearchParams = params as SearchParams;

    const searchRequest: SearchByQueryRequest = {
      type: 'query',
      searchTarget: SearchTarget.Video,
      query: searchParams.query,
      sort: null,
      period: null,
    };

    this.store.dispatch(
      SearchAction.search({
        searchRequest,
        pageSize: this.pageSize,
      })
    );

    if (this.containerEl?.nativeElement) {
      this.containerEl.nativeElement.scrollTop = 0;
    }
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
    return this.store.select(selectSearchResults);
  }

  isItemVideo(item: SearchableItem): item is Video {
    return isVideo(item);
  }

  asVideo(item: SearchableItem) {
    return item as Video;
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
