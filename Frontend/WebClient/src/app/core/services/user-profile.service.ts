import {map, of, switchMap} from 'rxjs';
import {AuthService} from 'src/app/auth/services/auth.service';
import {Injectable} from '@angular/core';
import {Store} from '@ngrx/store';
import {selectUserProfileClient} from '../selectors/users';
import {UserProfileStatus} from '../models/users';

@Injectable()
export class UserProfileService {
  constructor(private authService: AuthService, private store: Store) {}

  get userReady$() {
    return this.authService.authInfo$.pipe(
      switchMap((authInfo) => {
        if (!authInfo) return of(false);

        return this.store.select(selectUserProfileClient(authInfo.sub)).pipe(
          map((client) => {
            if (!client) {
              return true;
            }

            return client.userProfile?.status === UserProfileStatus.Registered;
          })
        );
      })
    );
  }
}
