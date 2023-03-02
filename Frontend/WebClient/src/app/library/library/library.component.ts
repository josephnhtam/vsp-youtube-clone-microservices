import {map, of, switchMap} from 'rxjs';
import {AuthService} from 'src/app/auth/services/auth.service';
import {LibraryService} from './library.service';
import {Component, OnInit} from '@angular/core';
import {Store} from '@ngrx/store';
import {selectUserProfileClient} from 'src/app/core/selectors/users';

@Component({
  selector: 'app-library',
  templateUrl: './library.component.html',
  styleUrls: ['./library.component.css'],
  providers: [LibraryService],
})
export class LibraryComponent implements OnInit {
  constructor(
    private service: LibraryService,
    private authService: AuthService,
    private store: Store
  ) {}

  ngOnInit(): void {
    this.service.loadLibrary(10, 5, 5, 10).subscribe();
    this.service.getUserSubscriptionInfo().subscribe();
    this.service.getPublicVideosCount().subscribe();
  }

  get userWatchRecordVideos() {
    return this.service.userWatchRecords?.items.map((x) => x.video);
  }

  get waterLaterPlaylistVideos() {
    return this.service.watchLaterPlaylist?.items.map((x) => x.video);
  }

  get likedPlaylistVideos() {
    return this.service.likedPlaylist?.items.map((x) => x.video);
  }

  get playlistInfos() {
    return this.service.playlistInfos;
  }

  get hasMorePlaylistInfos() {
    return this.service.canLoadMorePlaylists();
  }

  get loadingMorePlaylistInfos() {
    return this.service.isLoadingMorePlaylists();
  }

  get userId$() {
    return this.authService.authInfo$.pipe(map((x) => x?.sub));
  }

  get userDisplayName$() {
    return this.userId$.pipe(
      switchMap((userId) => {
        if (userId != null) {
          return this.store
            .select(selectUserProfileClient(userId))
            .pipe(map((x) => x?.userProfile?.displayName));
        }
        return of(null);
      })
    );
  }

  get subscriptionsCount() {
    return this.service.userSubscriptionInfo?.subscriptionsCount;
  }

  get uploadsCount() {
    return this.service.publicVideosCount;
  }

  get likesCount() {
    return this.service.likedPlaylist?.itemsCount;
  }

  get isLoaded() {
    return this.service.loaded;
  }

  get watchLaterCount() {
    return this.service.watchLaterPlaylist?.itemsCount || 0;
  }

  get playlistsCount() {
    return this.service.playlistsCount || 0;
  }

  get likedCount() {
    return this.service.likedPlaylist?.itemsCount || 0;
  }

  loadMorePlaylistInfos() {
    this.service.loadMorePlaylists().subscribe();
  }
}
