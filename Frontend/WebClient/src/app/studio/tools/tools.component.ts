import {AuthService} from './../../auth/services/auth.service';
import {Component, OnInit} from '@angular/core';
import {map, Observable, of} from 'rxjs';
import {EditVideoDialogService} from '../services/edit-video-dialog.service';

@Component({
  selector: 'app-tools',
  templateUrl: './tools.component.html',
  styleUrls: ['./tools.component.css'],
})
export class ToolsComponent implements OnInit {
  constructor(
    public authService: AuthService,
    private createVideoDialog: EditVideoDialogService
  ) {}

  ngOnInit(): void {}

  get authInfo$() {
    return this.authService.authInfo$;
  }

  get userId$(): Observable<string | undefined> {
    return this.authInfo$.pipe(map((x) => x?.sub));
  }

  onClickUploadVideo() {
    this.createVideoDialog.openDialog();
    return false;
  }

  get isLoading$() {
    return of(false);
  }
}
