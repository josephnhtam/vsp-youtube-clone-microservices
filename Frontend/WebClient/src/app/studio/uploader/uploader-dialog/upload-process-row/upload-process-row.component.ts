import {EditVideoDialogService} from './../../../services/edit-video-dialog.service';
import {selectVideoClientById} from './../../../videos-management/selectors/index';
import {concatLatestFrom} from '@ngrx/effects';
import {map, of, take, tap} from 'rxjs';
import {selectUploadProcess} from './../../selectors/index';
import {Store} from '@ngrx/store';
import {Component, Input, OnInit} from '@angular/core';
import {UploadStatus} from '../../models';
import {UploaderAction} from '../../actions';

@Component({
  selector: 'app-upload-process-row',
  templateUrl: './upload-process-row.component.html',
  styleUrls: ['./upload-process-row.component.css'],
})
export class UploadProcessRowComponent implements OnInit {
  @Input() uploadToken!: string;

  constructor(
    private store: Store,
    private dialogService: EditVideoDialogService
  ) {}

  ngOnInit(): void {}

  cancel() {
    this.store.dispatch(
      UploaderAction.cancelUpload({ uploadToken: this.uploadToken })
    );
  }

  edit() {
    this.process$
      .pipe(
        take(1),

        concatLatestFrom((process) => {
          if (process?.request.type === 'video') {
            return this.store.select(
              selectVideoClientById(process.request.videoId)
            );
          } else {
            return of(null);
          }
        }),

        tap(([process, videoClient]) => {
          if (!!videoClient) {
            this.dialogService.openDialog((process as any).request.videoId);
          }
        })
      )
      .subscribe();
  }

  get canEdit$() {
    return this.process$.pipe(
      take(1),

      concatLatestFrom((process) => {
        if (process?.request.type === 'video') {
          return this.store.select(
            selectVideoClientById(process.request.videoId)
          );
        } else {
          return of(null);
        }
      }),

      map(([_, videoClient]) => {
        return !!videoClient;
      })
    );
  }

  get process$() {
    return this.store.select(selectUploadProcess(this.uploadToken));
  }

  get fileName$() {
    return this.process$.pipe(map((x) => x?.request.file.name));
  }

  get isComplete$() {
    return this.process$.pipe(map((x) => x?.status == UploadStatus.Successful));
  }

  get canCancel$() {
    return this.process$.pipe(
      map((x) => {
        return x?.status == UploadStatus.InProgress;
      })
    );
  }

  get status$() {
    return this.process$.pipe(
      map((x) => {
        if (!x) {
          return '';
        }

        switch (x.status) {
          case UploadStatus.Successful:
            return '100% uploaded';

          case UploadStatus.InProgress:
            return x.progress > 0 ? `${x.progress}% uploaded` : 'Waiting...';

          case UploadStatus.Failed:
            return 'Upload failed';

          case UploadStatus.Cancelled:
            return 'Upload cancelled';
        }
      })
    );
  }
}
