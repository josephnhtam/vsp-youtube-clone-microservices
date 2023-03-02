import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-video-unavailable',
  templateUrl: './video-unavailable.component.html',
  styleUrls: ['./video-unavailable.component.css'],
})
export class VideoUnavailableComponent {
  statusCode?: number;

  constructor(private router: Router) {
    this.statusCode =
      this.router.getCurrentNavigation()?.extras.state?.['statusCode'];
  }

  get description() {
    if (this.statusCode == 401 || this.statusCode == 403) {
      return 'This video is private.';
    } else {
      return 'This video is no longer available.';
    }
  }
}
