import {selectUserProfileClient} from '../../core/selectors/users';
import {Store} from '@ngrx/store';
import {Component, Input, OnChanges, SimpleChanges} from '@angular/core';
import {map, of, take, tap} from 'rxjs';
import {UsersAction} from 'src/app/core/actions';

@Component({
  selector: 'app-user-profile-thumbnail',
  templateUrl: './user-profile-thumbnail.component.html',
  styleUrls: ['./user-profile-thumbnail.component.css'],
})
export class UserProfileThumbnailComponent implements OnChanges {
  @Input() size = 40;
  @Input() userId?: string;
  @Input() fetchUserProfile = false;
  @Input() hasHyperlink = false;

  constructor(private store: Store) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (this.fetchUserProfile && changes['userId']) {
      this.userProfile$
        .pipe(
          take(1),
          tap((userProfileClient) => {
            if (
              !userProfileClient ||
              (!userProfileClient.userProfile && !userProfileClient.pending)
            ) {
              if (this.userId) {
                this.store.dispatch(
                  UsersAction.getUserProfile({ userId: this.userId })
                );
              }
            }
          })
        )
        .subscribe();
    }
  }

  get sizeStyle() {
    return {
      width: `${this.size}px`,
      height: `${this.size}px`,
    };
  }

  get userProfile$() {
    if (!this.userId) return of(undefined);

    return this.store.select(selectUserProfileClient(this.userId));
  }

  get userThunmbnailData$() {
    return this.userProfile$.pipe(
      map((userProfileClinet) => {
        return userProfileClinet?.userProfile ?? undefined;
      })
    );
  }
}
