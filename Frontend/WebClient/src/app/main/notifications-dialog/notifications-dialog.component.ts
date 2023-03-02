import {MatMenuTrigger} from '@angular/material/menu';
import {selectHasMoreNotificationMessages, selectNotificationMessages,} from '../../core/selectors/notification';
import {filter, map, take, tap} from 'rxjs';
import {Store} from '@ngrx/store';
import {Component, ElementRef, Input, OnDestroy, OnInit, ViewChild,} from '@angular/core';
import {NotificationAction} from '../../core/actions';
import {concatLatestFrom} from '@ngrx/effects';
import {selectNotificationState} from "../../core/selectors";

@Component({
  selector: 'app-notifications-dialog',
  templateUrl: './notifications-dialog.component.html',
  styleUrls: ['./notifications-dialog.component.css'],
})
export class NotificationsDialogComponent implements OnInit, OnDestroy {
  @Input() pageSize = 30;

  @ViewChild('bottomSection', { static: true })
  bottomSection!: ElementRef;

  menuTrigger?: MatMenuTrigger;
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
  }

  load(menuTrigger: MatMenuTrigger) {
    this.menuTrigger = menuTrigger;

    this.store.dispatch(
      NotificationAction.loadMessages({ pageSize: this.pageSize })
    );
  }

  showMoreMessages() {
    this.store.dispatch(NotificationAction.loadMoreMessages());
  }

  onObserveBottom(
    entries: IntersectionObserverEntry[],
    observer: IntersectionObserver
  ) {
    const [entry] = entries;

    if (entry.isIntersecting) {
      this.store
        .select(selectNotificationState)
        .pipe(
          take(1),
          filter((x) => x.loaded && !x.pending),
          concatLatestFrom(() => this.hasMoreMessages$),
          tap(([_, hasMoreMessages]) => {
            if (hasMoreMessages) {
              this.showMoreMessages();
            }
          })
        )
        .subscribe();
    }
  }

  get state$() {
    return this.store.select(selectNotificationState);
  }

  get loaded$() {
    return this.state$.pipe(map((state) => state.loaded));
  }

  get pending$() {
    return this.state$.pipe(map((state) => state.pending));
  }

  get messages$() {
    return this.store.select(selectNotificationMessages);
  }

  get hasMoreMessages$() {
    return this.store.select(selectHasMoreNotificationMessages);
  }
}
