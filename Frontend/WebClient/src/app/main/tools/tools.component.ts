import { NotificationsDialogComponent } from './../notifications-dialog/notifications-dialog.component';
import { Router } from '@angular/router';
import { AuthService } from './../../auth/services/auth.service';
import { Component, OnInit } from '@angular/core';
import { EMPTY, exhaustMap, map, Observable, of, take, filter } from 'rxjs';
import { MatMenuTrigger } from '@angular/material/menu';
import { NotificationToolService } from './notification-tool.service';

@Component({
  selector: 'app-tools',
  templateUrl: './tools.component.html',
  styleUrls: ['./tools.component.css'],
  providers: [NotificationToolService],
})
export class ToolsComponent implements OnInit {
  unreadNotificationMessagesCount: string | null = null;

  constructor(
    public authService: AuthService,
    private router: Router,
    private notificationToolService: NotificationToolService
  ) {}

  get isLoading$() {
    return of(false);
  }

  ngOnInit(): void {
    this.refreshUnreadNotificationMessageCount();
  }

  private refreshUnreadNotificationMessageCount() {
    this.authService.isAuthenticated$
      .pipe(
        take(1),
        filter((isAuthenticated) => !!isAuthenticated)
      )
      .subscribe(() => {
        this.notificationToolService
          .getUnreadNotificationMessageCount()
          .subscribe((count) => {
            if (!!count && count > 0) {
              if (count <= 10) {
                this.unreadNotificationMessagesCount = count.toString();
              } else {
                this.unreadNotificationMessagesCount = '9+';
              }
            } else {
              this.unreadNotificationMessagesCount = null;
            }
          });
      });
  }

  private resetUnreadNotificationMessageCount() {
    this.notificationToolService
      .resetUnreadNotificationMessageCount()
      .subscribe();
  }

  get isAuthenticated$() {
    return this.authService.isAuthenticated$;
  }

  get authInfo$() {
    return this.authService.authInfo$;
  }

  get userId$(): Observable<string | undefined> {
    return this.authInfo$.pipe(map((x) => x?.sub));
  }

  onClickUploadVideo() {
    this.router.navigate(['/studio', 'videos'], {
      state: {
        createVideo: true,
      },
    });
    return false;
  }

  onClickSignin() {
    this.authService.isAuthenticated$
      .pipe(
        take(1),
        exhaustMap((isAuthenticated) => {
          if (!isAuthenticated) {
            return this.authService.signinRedirect();
          } else {
            return EMPTY;
          }
        })
      )
      .subscribe();
    return false;
  }

  onClickSignout() {
    this.authService.isAuthenticated$
      .pipe(
        take(1),
        exhaustMap((isAuthenticated) => {
          if (isAuthenticated) {
            return this.authService.signoutRedirect();
          } else {
            return EMPTY;
          }
        })
      )
      .subscribe();
    return false;
  }

  notificationMenuOpened(
    notificationDialog: NotificationsDialogComponent,
    menuTrigger: MatMenuTrigger
  ) {
    this.unreadNotificationMessagesCount = null;
    this.resetUnreadNotificationMessageCount();
    notificationDialog.load(menuTrigger);
  }
}
