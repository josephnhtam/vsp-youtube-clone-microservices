import {ConfirmDialogService} from './../../../shared/confirm-dialog/confirm-dialog.service';
import {UserProfileService} from 'src/app/core/services/user-profile.service';
import {Component, Input, OnChanges, OnInit, SimpleChanges,} from '@angular/core';
import {map, of} from 'rxjs';
import {AuthService} from 'src/app/auth/services/auth.service';
import {NotificationType} from '../../../core/models/subscription';
import {ChannelData} from 'src/app/core/models/channel';
import {ActivatedRoute} from '@angular/router';
import {ChannelSubscriptionService} from 'src/app/core/services/channel-subscription.service';

@Component({
  selector: 'app-channel-info',
  templateUrl: './channel-info.component.html',
  styleUrls: ['./channel-info.component.css'],
})
export class ChannelInfoComponent implements OnInit, OnChanges {
  @Input() channel!: ChannelData;

  get subscriptionStatus() {
    return this.channel.subscriptionStatus;
  }

  constructor(
    private authService: AuthService,
    private userProfileService: UserProfileService,
    private service: ChannelSubscriptionService,
    private route: ActivatedRoute,
    private confirmDialog: ConfirmDialogService
  ) {}

  ngOnChanges(changes: SimpleChanges) {}

  ngOnInit(): void {}

  get isLoaded() {
    return !!this.subscriptionStatus;
  }

  get subscribersCount() {
    return this.subscriptionStatus?.subscribersCount ?? 0;
  }

  get isSubscribed() {
    return this.subscriptionStatus?.isSubscribed;
  }

  get isAuthenticated$() {
    return this.authService.isAuthenticated$;
  }

  get isUserReady$() {
    return this.userProfileService.userReady$;
  }

  get isMine$() {
    return this.authService.authInfo$.pipe(
      map((x) => x?.sub == this.channel.userProfile.id)
    );
  }

  get isNoneNotification() {
    return this.subscriptionStatus?.notificationType === NotificationType.None;
  }

  get isNormalNotification() {
    return (
      this.subscriptionStatus?.notificationType === NotificationType.Normal
    );
  }

  get isEnhancedNotification() {
    return (
      this.subscriptionStatus?.notificationType === NotificationType.Enhanced
    );
  }

  switchSubscription() {
    if (!this.subscriptionStatus) return;

    if (this.subscriptionStatus.isSubscribed) {
      this.unsubscribe();
    } else {
      this.subscribe();
    }
  }

  canSubscribe$() {
    if (!this.subscriptionStatus) return of(false);

    return this.authService.authInfo$.pipe(
      map((authInfo) => {
        return authInfo?.sub != this.channel.userProfile.id;
      })
    );
  }

  subscribe() {
    if (!this.subscriptionStatus || this.subscriptionStatus.isSubscribed)
      return;
    this.service.subscribe(this.channel.userProfile, NotificationType.Normal);
    this.subscriptionStatus.isSubscribed = true;
    this.subscriptionStatus.notificationType = NotificationType.Normal;
    this.subscriptionStatus.subscribersCount++;
  }

  unsubscribe() {
    if (!this.subscriptionStatus || !this.subscriptionStatus.isSubscribed)
      return;

    const doUnsubscribe = () => {
      this.service.unsubscribe(this.channel.userProfile.id);
      this.subscriptionStatus!.isSubscribed = false;
      this.subscriptionStatus!.subscribersCount = Math.max(
        0,
        this.subscriptionStatus!.subscribersCount - 1
      );
    };

    this.confirmDialog.openConfirmDialog(
      null,
      `<div>Unsubscribe from <strong>${this.channel.userProfile.displayName}</strong></div>`,
      null,
      doUnsubscribe,
      null,
      'Unsubscribe',
      'Cancel'
    );
  }

  changeToEnhancedNotification() {
    this.changeNotification(NotificationType.Enhanced);
  }

  changeToNormalNotification() {
    this.changeNotification(NotificationType.Normal);
  }

  changeToNoneNotification() {
    this.changeNotification(NotificationType.None);
  }

  changeNotification(notificationType: NotificationType) {
    if (
      !this.subscriptionStatus ||
      this.subscriptionStatus.notificationType == notificationType
    )
      return;
    this.service.changeNotificationType(
      this.channel.userProfile.id,
      notificationType
    );
    this.subscriptionStatus.notificationType = notificationType;
  }
}
