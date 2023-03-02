import {concatLatestFrom} from '@ngrx/effects';
import {NavigationStart, Router} from '@angular/router';
import {DialogRef} from '@angular/cdk/dialog';
import {selectUploadProcess} from './../selectors/index';
import {UploadProcess, UploadStatus} from './../models/index';
import {Store} from '@ngrx/store';
import {Component, OnDestroy, OnInit} from '@angular/core';
import {selectUploadProcesses} from '../selectors';
import {combineLatest, map, of, Subscription} from 'rxjs';

@Component({
  selector: 'app-uploader-dialog',
  templateUrl: './uploader-dialog.component.html',
  styleUrls: ['./uploader-dialog.component.css'],
})
export class UploaderDialogComponent implements OnInit, OnDestroy {
  minimized = false;

  uploadTokens: string[] = [];

  private uploadProcessesSub?: Subscription;
  private navigationSub?: Subscription;

  constructor(
    private store: Store,
    private dialogRef: DialogRef,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.createUploadProcessSub();
    this.createNavigationSub();
  }

  ngOnDestroy(): void {
    this.uploadProcessesSub?.unsubscribe();
    this.navigationSub?.unsubscribe();
  }

  private createNavigationSub() {
    this.navigationSub = this.router.events
      .pipe(concatLatestFrom(() => this.isUploading$))
      .subscribe(([ev, isUploading]) => {
        if (!isUploading && ev instanceof NavigationStart) {
          this.close();
        }
      });
  }

  private createUploadProcessSub() {
    this.uploadProcessesSub = this.store
      .select(selectUploadProcesses)
      .subscribe(this.onUploadProcesses.bind(this));
  }

  private onUploadProcesses(uploadProcesses: UploadProcess[]) {
    this.uploadTokens = this.uploadTokens.filter((token) =>
      uploadProcesses.some((p) => p.request.uploadToken == token)
    );

    for (let uploadProcess of uploadProcesses) {
      if (uploadProcess.request.type !== 'video') {
        continue;
      }

      if (uploadProcess.status != UploadStatus.InProgress) {
        continue;
      }

      if (
        this.uploadTokens.some(
          (token) => token === uploadProcess.request.uploadToken
        )
      ) {
        continue;
      }

      this.uploadTokens.unshift(uploadProcess.request.uploadToken);
    }
  }

  close() {
    this.dialogRef.close();
  }

  get canClose$() {
    if (this.uploadTokens.length == 0) {
      return of(true);
    }

    const source = this.uploadTokens.map((x) =>
      this.store.select(selectUploadProcess(x))
    );

    return combineLatest(source).pipe(
      map((processes) => {
        let uploadingCount = 0;

        for (let process of processes) {
          if (!process) continue;

          if (process.status == UploadStatus.InProgress) {
            uploadingCount++;
          }
        }

        if (uploadingCount == 0) {
          return true;
        } else {
          return false;
        }
      })
    );
  }

  get uploadingProgress$() {
    if (this.uploadTokens.length == 0) {
      return of(0);
    }

    const source = this.uploadTokens.map((x) =>
      this.store.select(selectUploadProcess(x))
    );

    return combineLatest(source).pipe(
      map((processes) => {
        let progress = 0;
        let uploadingCount = 0;

        for (let process of processes) {
          if (!process) continue;

          if (
            process.progress > 0 &&
            process.status == UploadStatus.InProgress
          ) {
            progress += process.progress;
            uploadingCount++;
          }
        }

        return progress / uploadingCount;
      })
    );
  }

  get isUploading$() {
    if (this.uploadTokens.length == 0) {
      return of(false);
    }

    const source = this.uploadTokens.map((x) =>
      this.store.select(selectUploadProcess(x))
    );

    return combineLatest(source).pipe(
      map((processes) => {
        for (let process of processes) {
          if (!process) continue;

          if (process.status == UploadStatus.InProgress) {
            return true;
          }
        }

        return false;
      })
    );
  }

  get title$() {
    if (this.uploadTokens.length == 0) {
      return of('No upload');
    }

    const source = this.uploadTokens.map((x) =>
      this.store.select(selectUploadProcess(x))
    );

    return combineLatest(source).pipe(
      map((processes) => {
        let uploadedCount = 0;
        let uploadingCount = 0;

        for (let process of processes) {
          if (!process) continue;

          if (process.status == UploadStatus.InProgress) {
            uploadingCount++;
          } else if (process.status == UploadStatus.Successful) {
            uploadedCount++;
          }
        }

        if (uploadingCount > 0) {
          const totalCount = uploadedCount + uploadingCount;
          return `Uploading ${uploadedCount} of ${totalCount}`;
        } else {
          if (uploadedCount > 1) {
            return 'Uploads complete';
          } else if (uploadedCount == 1) {
            return 'Upload complete';
          } else {
            return 'No upload';
          }
        }
      })
    );
  }
}
