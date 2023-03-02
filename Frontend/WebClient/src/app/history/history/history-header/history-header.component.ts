import {ConfirmDialogService} from '../../../shared/confirm-dialog/confirm-dialog.service';
import {ActivatedRoute, Router} from '@angular/router';
import {NgForm} from '@angular/forms';
import {Component, OnInit} from '@angular/core';
import {HistoryParams} from '../../../core/models/history';
import {UserHistorySettingsService} from '../user-history-settings.service';

@Component({
  selector: 'app-history-header',
  templateUrl: './history-header.component.html',
  styleUrls: ['./history-header.component.css'],
})
export class HistoryHeaderComponent implements OnInit {
  constructor(
    private service: UserHistorySettingsService,
    private router: Router,
    private route: ActivatedRoute,
    private confirmDialog: ConfirmDialogService
  ) {}

  ngOnInit(): void {}

  get recordUserWatchHistoryEnabled() {
    return this.service.settings?.recordWatchHistory;
  }

  search(form: NgForm) {
    const query = form.value['query'] ?? '';

    const params: HistoryParams = {
      query,
    };

    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: params,
    });
  }

  clearWatchHistory() {
    const doClear = () => {
      this.service.clearUserWatchHistory();
    };

    this.confirmDialog.openConfirmDialog(
      'Clear watch history?',
      `<div>Your watch history will be cleared.</div>
       <div class="note">Note: Clearing watch history is a permanent action and cannot be undone.</div>`,
      null,
      doClear,
      null,
      'Clear watch history',
      'Cancel'
    );
  }

  turnOnWatchHistory() {
    this.service.switchRecordUserWatchHistory(true).subscribe();
  }

  pauseWatchHistory() {
    this.service.switchRecordUserWatchHistory(false).subscribe();
  }
}
