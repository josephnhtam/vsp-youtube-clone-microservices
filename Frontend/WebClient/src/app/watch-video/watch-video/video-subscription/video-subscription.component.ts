import {ConfirmDialogService} from '../../../shared/confirm-dialog/confirm-dialog.service';
import {UserProfileService} from '../../../core/services/user-profile.service';
import {Component, Input, OnChanges, OnInit, SimpleChanges,} from '@angular/core';
import {map, of} from 'rxjs';
import {AuthService} from 'src/app/auth/services/auth.service';
import {NotificationType, SubscriptionStatus,} from '../../../core/models/subscription';
import {Video} from '../../../core/models/video';
import {getChannelLink} from 'src/app/core/services/utilities';
import {ChannelSubscriptionService} from 'src/app/core/services/channel-subscription.service';

@Component({
  selector: 'app-video-subscription',
  templateUrl: './video-subscription.component.html',
  styleUrls: ['./video-subscription.component.css'],
})
export class VideoSubscriptionComponent implements OnInit, OnChanges {
  @Input()
  video!: Video;

  subscriptionStatus?: SubscriptionStatus;

  constructor(
    private service: ChannelSubscriptionService,
    private authService: AuthService,
    private userProfileService: UserProfileService,
    private confirmDialog: ConfirmDialogService
  ) {}

  ngOnChanges(changes: SimpleChanges) {
    this.subscriptionStatus = undefined;
    this.service.getSubscriptionStatus(this.video.creatorProfile.id).subscribe({
      next: (subscriptionStatus) => {
        this.subscriptionStatus = subscriptionStatus;
      },
    });
  }

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

  get channelLink() {
    return getChannelLink(this.video.creatorProfile);
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
        return authInfo?.sub != this.video.creatorProfile.id;
      })
    );
  }

  subscribe() {
    if (!this.subscriptionStatus || this.subscriptionStatus.isSubscribed)
      return;
    this.service.subscribe(this.video.creatorProfile, NotificationType.Normal);
    this.subscriptionStatus.isSubscribed = true;
    this.subscriptionStatus.notificationType = NotificationType.Normal;
    this.subscriptionStatus.subscribersCount++;
  }

  unsubscribe() {
    if (!this.subscriptionStatus || !this.subscriptionStatus.isSubscribed)
      return;

    const doUnsubscribe = () => {
      this.service.unsubscribe(this.video.creatorProfile.id);
      this.subscriptionStatus!.isSubscribed = false;
      this.subscriptionStatus!.subscribersCount = Math.max(
        0,
        this.subscriptionStatus!.subscribersCount - 1
      );
    };

    this.confirmDialog.openConfirmDialog(
      null,
      `<div>Unsubscribe from <strong>${this.video.creatorProfile.displayName}</strong></div>`,
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
      this.video.creatorProfile.id,
      notificationType
    );
    this.subscriptionStatus.notificationType = notificationType;
  }
}
