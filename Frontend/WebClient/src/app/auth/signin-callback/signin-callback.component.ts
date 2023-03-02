import {Router} from '@angular/router';
import {AuthService} from './../services/auth.service';
import {Component, OnInit} from '@angular/core';

@Component({
  selector: 'app-signin-callback',
  templateUrl: './signin-callback.component.html',
  styleUrls: ['./signin-callback.component.css'],
})
export class SigninCallbackComponent implements OnInit {
  constructor(private authService: AuthService, private router: Router) {}

  async ngOnInit() {
    console.log('Verifying Authorization Code');

    try {
      await this.authService.validateAuthorizationCode();
      console.log('Logged in');
      this.router.navigate(['/'], {
        replaceUrl: true,
      });
    } catch (err) {
      console.error(err);
      this.router.navigate(['/']);
    }
  }
}
