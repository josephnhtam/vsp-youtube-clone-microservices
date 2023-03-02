import {Component, ElementRef, OnDestroy, OnInit, ViewChild,} from '@angular/core';
import {Store} from '@ngrx/store';
import {SearchAction} from '../../core/actions';
import {filter, map, take, tap} from 'rxjs';
import {selectHasMoreSearchResults, selectSearchResults,} from '../../core/selectors/search';
import {isVideo, Video} from '../../core/models/search';
import {concatLatestFrom} from '@ngrx/effects';
import {selectSearchState} from "../../core/selectors";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent implements OnInit, OnDestroy {
  tags?: string[];

  pageSize: number = 24;

  @ViewChild('bottomSection', { static: true })
  bottomSection!: ElementRef;

  private bottomSectionObserver: IntersectionObserver;

  constructor(private store: Store) {
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
    this.store.dispatch(
      SearchAction.searchTrendingVideos({ pageSize: this.pageSize })
    );

    this.bottomSectionObserver.observe(this.bottomSection.nativeElement);
  }

  get searchState$() {
    return this.store.select(selectSearchState);
  }

  get searchResults$() {
    return this.store.select(selectSearchResults).pipe(
      map((results) => {
        return results.filter((x) => isVideo(x)).map((x) => x as Video);
      })
    );
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
