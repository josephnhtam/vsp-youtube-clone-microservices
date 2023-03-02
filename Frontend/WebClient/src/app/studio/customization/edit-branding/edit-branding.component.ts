import {BrandingUpdate, UserData} from './../models/index';
import {Subscription, take, tap} from 'rxjs';
import {Component, OnDestroy, OnInit} from '@angular/core';
import {Store} from '@ngrx/store';
import {selectUserData} from '../selectors';

@Component({
  selector: 'app-edit-branding',
  templateUrl: './edit-branding.component.html',
  styleUrls: ['./edit-branding.component.css'],
})
export class EditBrandingComponent implements OnInit, OnDestroy {
  userData!: UserData;

  thumbnailFile: File | null = null;
  bannerFile: File | null = null;

  thumbnailUrl: string | null = null;
  bannerUrl: string | null = null;

  update!: BrandingUpdate;

  get thumbnailAccepted() {
    return this.thumbnailUrl != null && this.thumbnailUrl != '';
  }
  thumbnailMimeTypes = ['image/png', 'image/jpeg'];
  thumbnailExtensions = '.png,.jpg,.jpeg';
  thumbnailMaxSize = 4096;

  get bannerAccepted() {
    return this.bannerUrl != null && this.bannerUrl != '';
  }
  bannerMimeTypes = ['image/png', 'image/jpeg'];
  bannerExtensions = '.png,.jpg,.jpeg';
  bannerMaxSize = 6144;

  private userDataUpdatedSub?: Subscription;

  constructor(private store: Store) {}

  ngOnDestroy(): void {
    this.userDataUpdatedSub?.unsubscribe();
  }

  ngOnInit(): void {
    this.userDataUpdatedSub = this.store
      .select(selectUserData)
      .pipe(
        tap(() => {
          this.reset();
        })
      )
      .subscribe();
  }

  reset() {
    this.store
      .select(selectUserData)
      .pipe(
        take(1),
        tap((userData) => {
          if (userData != null) {
            this.syncUiWithUserData(userData);
          }
        })
      )
      .subscribe();

    this.clearDirty();
  }

  syncUiWithUserData(userData: UserData) {
    this.userData = userData;
    this.thumbnailUrl = userData.userProfile.thumbnailUrl;
    this.bannerUrl = userData.userChannel.bannerUrl;
  }

  setThumbnailToUpload(file: File) {
    this.thumbnailUrl = null;

    const fr = new FileReader();
    fr.readAsDataURL(file);
    fr.onload = () => {
      this.thumbnailUrl = fr.result as string;
    };

    this.thumbnailFile = file;
    this.update.thumbnailChanged = true;
    this.update.newThubmnailFile = file;
  }

  thumbnailRemoved() {
    this.thumbnailUrl = null;

    if (
      this.userData.userProfile.thumbnailUrl != null &&
      this.userData.userProfile.thumbnailUrl != ''
    ) {
      this.update.thumbnailChanged = true;
      this.update.newThubmnailFile = null;
    }
  }

  setBannerToUpload(file: File) {
    this.bannerUrl = null;

    const fr = new FileReader();
    fr.readAsDataURL(file);
    fr.onload = () => {
      this.bannerUrl = fr.result as string;
    };

    this.bannerFile = file;
    this.update.bannerChanged = true;
    this.update.newBannerFile = file;
  }

  bannerRemoved() {
    this.bannerUrl = null;

    if (
      this.userData.userChannel.bannerUrl != null &&
      this.userData.userChannel.bannerUrl != ''
    ) {
      this.update.bannerChanged = true;
      this.update.newBannerFile = null;
    }
  }

  isValid() {
    return true;
  }

  isDirty() {
    return this.update.thumbnailChanged || this.update.bannerChanged;
  }

  getUpdate(): BrandingUpdate {
    return this.update;
  }

  clearDirty() {
    this.update = {
      thumbnailChanged: false,
      bannerChanged: false,
      newThubmnailFile: null,
      newBannerFile: null,
    };
  }
}
