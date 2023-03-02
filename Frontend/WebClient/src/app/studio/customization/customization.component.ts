import {concatLatestFrom} from '@ngrx/effects';
import {UsersAction} from 'src/app/core/actions';
import {concatMap, filter, map, of, take} from 'rxjs';
import {AuthService} from 'src/app/auth/services/auth.service';
import {Router} from '@angular/router';
import {EditLayoutComponent} from './edit-layout/edit-layout.component';
import {selectCustomizationPending} from './selectors/index';
import {Component, OnInit, ViewChild} from '@angular/core';
import {Store} from '@ngrx/store';
import {CustomizationAction} from './actions';
import {EditBasicInfoComponent} from './edit-basic-info/edit-basic-info.component';
import {EditBrandingComponent} from './edit-branding/edit-branding.component';
import {selectUserProfileClient} from 'src/app/core/selectors/users';
import {getChannelLink} from 'src/app/core/services/utilities';

@Component({
  selector: 'app-customization',
  templateUrl: './customization.component.html',
  styleUrls: ['./customization.component.css'],
})
export class CustomizationComponent implements OnInit {
  @ViewChild('layout') layout!: EditLayoutComponent;
  @ViewChild('branding') branding!: EditBrandingComponent;
  @ViewChild('basicInfo') basicInfo!: EditBasicInfoComponent;

  constructor(
    private store: Store,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {}

  isDirty() {
    return (
      this.layout?.isDirty() ||
      this.branding?.isDirty() ||
      this.basicInfo?.isDirty()
    );
  }

  isValid() {
    return (
      (this.layout?.isValid() ?? true) &&
      (this.branding?.isValid() ?? true) &&
      (this.basicInfo?.isValid() ?? true)
    );
  }

  get isPending$() {
    return this.store.select(selectCustomizationPending);
  }

  reset() {
    this.layout?.reset();
    this.branding?.reset();
    this.basicInfo?.reset();
    return false;
  }

  publish() {
    const layoutUpdate = this.layout.getUpdate();
    const brandingUpdate = this.branding.getUpdate();
    const basicInfoUpdate = this.basicInfo.getUpdate();

    this.store.dispatch(
      CustomizationAction.updateUser({
        layoutUpdate,
        brandingUpdate,
        basicInfoUpdate,
      })
    );

    this.store
      .select(selectCustomizationPending)
      .pipe(
        filter((pending) => !pending),
        take(1),
        concatLatestFrom(() => this.authService.authInfo$)
      )
      .subscribe(([_, authInfo]) => {
        if (authInfo) {
          this.store.dispatch(
            UsersAction.getUserProfile({
              userId: authInfo.sub,
            })
          );
        }
      });

    return false;
  }

  get channelLink$() {
    return this.authService.authInfo$.pipe(
      concatMap((authInfo) => {
        if (!authInfo) return of([]);

        return this.store.select(selectUserProfileClient(authInfo.sub)).pipe(
          map((userProfileClient) => {
            if (!userProfileClient?.userProfile) return [];

            return getChannelLink(userProfileClient.userProfile);
          })
        );
      })
    );
  }
}
