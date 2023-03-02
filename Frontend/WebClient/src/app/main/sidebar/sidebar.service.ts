import {selectSubscriptionInfos} from './../../core/selectors/subscription-mangament';
import {PlaylistManagementAction, SubscriptionManagementAction,} from 'src/app/core/actions';
import {Store} from '@ngrx/store';
import {AuthService} from 'src/app/auth/services/auth.service';
import {UserProfileService} from './../../core/services/user-profile.service';
import {Subscription} from 'rxjs';
import {Injectable, OnDestroy} from '@angular/core';
import {
  selectIsRetrievingSimplePlaylistInfos,
  selectSimplePlaylistInfos,
} from 'src/app/core/selectors/playlist-management';
import {selectIsRetrievingSubscriptionInfos} from 'src/app/core/selectors/subscription-mangament';

@Injectable()
export class SidebarService implements OnDestroy {
  onAuthenticatedSub?: Subscription;
  onUserReadySub?: Subscription;
  wasReady = false;

  constructor(
    private authService: AuthService,
    private userProfileService: UserProfileService,
    private store: Store
  ) {}

  initialize() {
    this.onAuthenticatedSub = this.authService.isAuthenticated$.subscribe(
      this.onAuthenticated.bind(this)
    );

    this.onUserReadySub = this.userProfileService.userReady$.subscribe(
      this.onUserReady.bind(this)
    );
  }

  ngOnDestroy(): void {
    this.onAuthenticatedSub?.unsubscribe();
    this.onUserReadySub?.unsubscribe();
  }

  private onAuthenticated(isAuthenticated: boolean) {
    if (!isAuthenticated) {
      this.clearPlaylistInfos();
      this.clearSubscriptionInfos();
    }
  }

  private onUserReady(isReady: boolean) {
    if (!isReady || this.wasReady) {
      return;
    }

    this.wasReady = true;

    this.retrievePlaylistInfos();
    this.retrieveSubscriptionInfos();
  }

  get isRetrievingPlaylistInfos$() {
    return this.store.select(selectIsRetrievingSimplePlaylistInfos);
  }

  get isRetrievingSubscriptionInfos$() {
    return this.store.select(selectIsRetrievingSubscriptionInfos);
  }

  retrievePlaylistInfos() {
    this.store.dispatch(
      PlaylistManagementAction.retrieveSimplePlaylistInfos({ maxCount: 100 })
    );
  }

  retrieveSubscriptionInfos() {
    this.store.dispatch(
      SubscriptionManagementAction.retrieveSubscriptions({ maxCount: 200 })
    );
  }

  clearPlaylistInfos() {
    this.store.dispatch(PlaylistManagementAction.clearSimplePlaylistInfos());
  }

  clearSubscriptionInfos() {
    this.store.dispatch(SubscriptionManagementAction.clearSubscriptions());
  }

  get playlistInfos$() {
    return this.store.select(selectSimplePlaylistInfos);
  }

  get subscriptionInfos$() {
    return this.store.select(selectSubscriptionInfos);
  }
}
