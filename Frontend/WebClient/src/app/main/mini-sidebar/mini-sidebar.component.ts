import {Component, OnInit} from '@angular/core';
import {UserProfileService} from 'src/app/core/services/user-profile.service';

@Component({
  selector: 'app-mini-sidebar',
  templateUrl: './mini-sidebar.component.html',
  styleUrls: ['./mini-sidebar.component.css'],
})
export class MiniSidebarComponent implements OnInit {
  constructor(private userProfileService: UserProfileService) {}

  ngOnInit(): void {}

  get isUserReady$() {
    return this.userProfileService.userReady$;
  }
}
