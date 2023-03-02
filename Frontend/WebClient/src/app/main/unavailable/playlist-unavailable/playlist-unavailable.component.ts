import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-playlist-unavailable',
  templateUrl: './playlist-unavailable.component.html',
  styleUrls: ['./playlist-unavailable.component.css'],
})
export class PlaylistUnavailableComponent {
  statusCode?: number;

  constructor(private router: Router) {
    this.statusCode =
      this.router.getCurrentNavigation()?.extras.state?.['statusCode'];
  }

  get description() {
    if (this.statusCode == 401 || this.statusCode == 403) {
      return 'This playlist is private.';
    } else {
      return 'This playlist is no longer available.';
    }
  }
}
