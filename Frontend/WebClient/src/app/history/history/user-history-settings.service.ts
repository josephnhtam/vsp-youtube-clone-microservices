import { tap } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { UserHistorySettings } from '../../core/models/history';
import { Injectable } from '@angular/core';
import { Store } from '@ngrx/store';
import { HistoryAction } from '../../core/actions';

@Injectable()
export class UserHistorySettingsService {
  private _settings?: UserHistorySettings;

  constructor(private store: Store, private httpClient: HttpClient) {}

  updateSettings(settings: UserHistorySettings) {
    this._settings = settings;
  }

  get settings() {
    if (!this._settings) return null;

    return {
      ...this._settings,
    };
  }

  switchRecordUserWatchHistory(enable: boolean) {
    const url =
      environment.appSetup.apiUrl + '/api/v1/UserHistory/EnableRecording';

    return this.httpClient
      .post(url, {
        enable,
      })
      .pipe(
        tap(() => {
          if (!this._settings) return;
          this._settings.recordWatchHistory = enable;
        })
      );
  }

  clearUserWatchHistory() {
    this.store.dispatch(HistoryAction.clearHistory());
  }
}
