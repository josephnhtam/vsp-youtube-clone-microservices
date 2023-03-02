import {EditVideoDialogService} from './services/edit-video-dialog.service';
import {Router} from '@angular/router';
import {Component, OnInit} from '@angular/core';

@Component({
  selector: 'app-studio',
  templateUrl: './studio.component.html',
  styleUrls: ['./studio.component.css'],
})
export class StudioComponent implements OnInit {
  hideSidebar = false;

  constructor(
    private router: Router,
    private createVideoDialog: EditVideoDialogService
  ) {
    const navigation = this.router.getCurrentNavigation();
    if (navigation?.extras.state?.['createVideo']) {
      this.createVideoDialog.openDialog();
    }
  }

  ngOnInit(): void {}
}
