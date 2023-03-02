import {UserData} from './../models/index';
import {of, Subscription, take, tap} from 'rxjs';
import {Component, OnDestroy, OnInit} from '@angular/core';
import {BasicInfoUpdate} from '../models';
import {Store} from '@ngrx/store';
import {selectUserData} from '../selectors';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';

@Component({
  selector: 'app-edit-basic-info',
  templateUrl: './edit-basic-info.component.html',
  styleUrls: ['./edit-basic-info.component.css'],
})
export class EditBasicInfoComponent implements OnInit, OnDestroy {
  userData!: UserData;
  formGroup!: FormGroup;

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
            this.userData = userData;
            this.syncUiWithUserData(userData);
          }
        })
      )
      .subscribe();
  }

  syncUiWithUserData(userData: UserData) {
    const fb = new FormBuilder();

    this.formGroup = fb.group({
      displayName: [
        userData.userProfile.displayName,
        [Validators.required, Validators.maxLength(50)],
      ],
      description: [
        userData.userProfile.description,
        [Validators.maxLength(1000)],
      ],
      handle: [userData.userProfile.handle ?? '', [Validators.maxLength(30)]],
      email: [userData.userProfile.email ?? '', [Validators.maxLength(255)]],
    });
  }

  get handleHint$() {
    return of('');
  }

  isValid() {
    return this.formGroup.valid;
  }

  isDirty() {
    if (!this.formGroup.dirty) {
      return false;
    }

    const value = this.formGroup.value;
    if (value['displayName'] != this.userData.userProfile.displayName) {
      return true;
    }

    if (value['description'] != this.userData.userProfile.description) {
      return true;
    }

    if (value['email'] != (this.userData.userProfile.email ?? '')) {
      return true;
    }

    if (value['handle'] != (this.userData.userProfile.handle ?? '')) {
      return true;
    }

    return false;
  }

  getUpdate(): BasicInfoUpdate | null {
    if (this.isValid() && this.isDirty()) {
      const value = this.formGroup.value;
      return {
        displayName: value['displayName'],
        description: value['description'],
        email: value['email'],
        handle: value['handle'],
      };
    }

    return null;
  }

  clearDirty() {
    this.formGroup.markAsPristine();
  }
}
