import {UserData} from './models/index';
import {Actions, ofType} from '@ngrx/effects';
import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, Resolve, Router, RouterStateSnapshot,} from '@angular/router';
import {Store} from '@ngrx/store';
import {map, take} from 'rxjs';
import {CustomizationAction, CustomizationApiAction} from './actions';

@Injectable()
export class UserDataResolver implements Resolve<UserData | null> {
  constructor(
    private store: Store,
    private actions$: Actions,
    private router: Router
  ) {}

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    this.store.dispatch(CustomizationAction.retrieveUserData());

    return this.actions$.pipe(
      ofType(
        CustomizationApiAction.userDataRetrieved,
        CustomizationApiAction.failedToRetrieveUserData
      ),

      take(1),

      map((action) => {
        if (
          action.type === CustomizationApiAction.failedToRetrieveUserData.type
        ) {
          this.router.navigate(['/']);
          return null;
        }

        return action.userData;
      })
    );
  }
}
