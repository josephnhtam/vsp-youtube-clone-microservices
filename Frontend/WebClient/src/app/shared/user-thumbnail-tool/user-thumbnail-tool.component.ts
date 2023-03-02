import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {Store} from '@ngrx/store';
import {EMPTY, exhaustMap, map, Observable, of, switchMap, take,} from 'rxjs';
import {AuthService} from 'src/app/auth/services/auth.service';
import {selectUserProfileClient} from 'src/app/core/selectors/users';
import {getChannelLink} from '../../core/services/utilities';

@Component({
  selector: 'app-user-thumbnail-tool',
  templateUrl: './user-thumbnail-tool.component.html',
  styleUrls: ['./user-thumbnail-tool.component.css'],
})
export class UserThumbnailToolComponent implements OnInit {
  constructor(
    public authService: AuthService,
    private router: Router,
    private store: Store
  ) {}

  ngOnInit(): void {}

  get isAuthenticated$() {
    return this.authService.isAuthenticated$;
  }

  get authInfo$() {
    return this.authService.authInfo$;
  }

  get userId$(): Observable<string | undefined> {
    return this.authInfo$.pipe(map((x) => x?.sub));
  }

  get userProfile$() {
    return this.userId$.pipe(
      switchMap((userId) => {
        if (!!userId) {
          return this.store
            .select(selectUserProfileClient(userId))
            .pipe(map((x) => x?.userProfile));
        } else {
          return of(null);
        }
      })
    );
  }

  get channelLink$() {
    return this.userProfile$.pipe(
      map((userProfile) => {
        if (!!userProfile) {
          return getChannelLink(userProfile);
        }
        return null;
      })
    );
  }

  onClickSignin() {
    this.authService.isAuthenticated$
      .pipe(
        take(1),
        exhaustMap((isAuthenticated) => {
          if (!isAuthenticated) {
            return this.authService.signinRedirect();
          } else {
            return EMPTY;
          }
        })
      )
      .subscribe();
    return false;
  }

  onClickSignout() {
    this.authService.isAuthenticated$
      .pipe(
        take(1),
        exhaustMap((isAuthenticated) => {
          if (isAuthenticated) {
            return this.authService.signoutRedirect();
          } else {
            return EMPTY;
          }
        })
      )
      .subscribe();
    return false;
  }
}
