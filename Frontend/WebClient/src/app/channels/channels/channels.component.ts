import {Component, ElementRef, OnDestroy, OnInit, ViewChild,} from '@angular/core';
import {Store} from '@ngrx/store';
import {filter, map, take, tap} from 'rxjs';
import {concatLatestFrom} from '@ngrx/effects';
import {selectHasMoreSubscriptions, selectSubscriptions,} from '../../core/selectors/subscription-listing';
import {SubscriptionListingAction} from '../../core/actions';
import {selectSubscriptionListingState} from "../../core/selectors";
import {SubscriptionTargetSort} from "../../core/models/subscription";

@Component({
  selector: 'app-channels',
  templateUrl: './channels.component.html',
  styleUrls: ['./channels.component.css'],
})
export class ChannelsComponent implements OnInit, OnDestroy {
  pageSize: number = 50;
  sort: SubscriptionTargetSort = SubscriptionTargetSort.DisplayName;

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
    this.bottomSectionObserver.observe(this.bottomSection.nativeElement);

    this.getUserSubscriptions();
  }

  getUserSubscriptions() {
    this.store.dispatch(
      SubscriptionListingAction.getSubscriptions({
        pageSize: this.pageSize,
        sort: SubscriptionTargetSort.DisplayName,
      })
    );
  }

  onObserveBottom(
    entries: IntersectionObserverEntry[],
    observer: IntersectionObserver
  ) {
    const [entry] = entries;

    if (entry.isIntersecting) {
      this.store
        .select(selectSubscriptionListingState)
        .pipe(
          take(1),
          filter((x) => x.loaded && !x.pending && !x.loadingMoreSubscriptions),
          concatLatestFrom(() => this.hasMoreSubscriptions$),
          tap(([_, hasMoreSubscriptions]) => {
            if (hasMoreSubscriptions) {
              this.showMoreSubscriptions();
            }
          })
        )
        .subscribe();
    }
  }

  get hasMoreSubscriptions$() {
    return this.store.select(selectHasMoreSubscriptions);
  }

  showMoreSubscriptions() {
    this.store.dispatch(SubscriptionListingAction.getMoreSubscriptions());
  }

  get subscriptions$() {
    return this.store.select(selectSubscriptions);
  }

  get initializing$() {
    return this.store
      .select(selectSubscriptionListingState)
      .pipe(map((x) => x.pending && !x.loaded));
  }

  get isLoaded$() {
    return this.store
      .select(selectSubscriptionListingState)
      .pipe(map((x) => x.loaded));
  }

  get loadingMoreSubscriptions$() {
    return this.store
      .select(selectSubscriptionListingState)
      .pipe(map((x) => x.loadingMoreSubscriptions));
  }
}
