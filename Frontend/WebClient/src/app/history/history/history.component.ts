import {selectHasMoreHistoryRecords, selectHistoryRecords,} from '../../core/selectors/history';
import {HistoryParams, SearchUserWatchHistoryRequest,} from '../../core/models/history';
import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {ActivatedRoute, Params} from '@angular/router';
import {Store} from '@ngrx/store';
import {filter, map, Subscription, take, tap} from 'rxjs';
import {HistoryAction} from '../../core/actions';
import {concatLatestFrom} from '@ngrx/effects';
import {selectHistoryState} from "../../core/selectors";

@Component({
  selector: 'app-history',
  templateUrl: './history.component.html',
  styleUrls: ['./history.component.css'],
})
export class HistoryComponent implements OnInit {
  pageSize: number = 30;

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
    const searchParams: HistoryParams = params as HistoryParams;

    const searchRequest: SearchUserWatchHistoryRequest = {
      query: searchParams.query,
      period: null,
    };

    this.store.dispatch(
      HistoryAction.search({
        searchRequest,
        pageSize: this.pageSize,
      })
    );
  }

  get noRecordsFound$() {
    return this.historyState$.pipe(
      map((x) => (x.loaded || x.error) && x.ids.length == 0)
    );
  }

  get historyState$() {
    return this.store.select(selectHistoryState);
  }

  get historyRecords$() {
    return this.store.select(selectHistoryRecords);
  }

  get historyDateTimes$() {
    return this.historyRecords$.pipe(
      map((records) => {
        let times: number[] = [];
        let timeSet = new Set();

        for (let record of records) {
          const date = new Date(record.parsedDate);
          date.setHours(0, 0, 0, 0);
          timeSet.add(date.getTime());
        }

        for (const time of timeSet.keys()) {
          times.push(Number(time));
        }

        times = times.sort((a, b) => b - a);
        return times;
      })
    );
  }

  get isLoaded$() {
    return this.historyState$.pipe(map((x) => x.loaded));
  }

  get isLoadingMoreRecords$() {
    return this.historyState$.pipe(map((x) => x.loadingMoreRecords));
  }

  get hasMoreRecords$() {
    return this.store.select(selectHasMoreHistoryRecords);
  }

  loadMoreRecords() {
    this.store.dispatch(HistoryAction.loadMoreRecords());
  }

  onObserveBottom(
    entries: IntersectionObserverEntry[],
    observer: IntersectionObserver
  ) {
    const [entry] = entries;

    if (entry.isIntersecting) {
      this.historyState$
        .pipe(
          take(1),
          filter((x) => x.loaded && !x.pending && !x.loadingMoreRecords),
          concatLatestFrom(() => this.hasMoreRecords$),
          tap(([_, hasMoreRecords]) => {
            if (hasMoreRecords) {
              this.loadMoreRecords();
            }
          })
        )
        .subscribe();
    }
  }
}
