import {Component, Input, OnInit} from '@angular/core';
import {ChannelSubscriptionService} from 'src/app/core/services/channel-subscription.service';
import {getChannelLink} from 'src/app/core/services/utilities';
import {ConfirmDialogService} from 'src/app/shared/confirm-dialog/confirm-dialog.service';
import {NotificationType, SubscriptionClient,} from '../../../core/models/subscription';

@Component({
  selector: 'app-channel-row',
  templateUrl: './channel-row.component.html',
  styleUrls: ['./channel-row.component.css'],
})
export class ChannelRowComponent implements OnInit {
  @Input()
  subscription!: SubscriptionClient;

  constructor(
    private service: ChannelSubscriptionService,
    private confirmDialog: ConfirmDialogService
  ) {}

  ngOnInit(): void {}

  get subscribersCount() {
    return this.subscription.userProfile.subscribersCount ?? 0;
  }

  get isSubscribed() {
    return this.subscription.isSubscribed;
  }

  get isNoneNotification() {
    return this.subscription.notificationType === NotificationType.None;
  }

  get isNormalNotification() {
    return this.subscription.notificationType === NotificationType.Normal;
  }

  get isEnhancedNotification() {
    return this.subscription.notificationType === NotificationType.Enhanced;
  }

  get channelLink() {
    return getChannelLink(this.subscription.userProfile);
  }

  subscribe() {
    if (this.subscription.isSubscribed) {
      return;
    }

    this.service.subscribe(
      this.subscription.userProfile,
      NotificationType.Normal
    );
  }

  unsubscribe() {
    if (!this.subscription.isSubscribed) {
      return;
    }

    const doUnsubscribe = () => {
      this.service.unsubscribe(this.subscription.userProfile.id);
    };

    this.confirmDialog.openConfirmDialog(
      null,
      `<div>Unsubscribe from <strong>${this.subscription.userProfile.displayName}</strong></div>`,
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
    if (this.subscription.notificationType == notificationType) {
      return;
    }

    this.service.changeNotificationType(
      this.subscription.userProfile.id,
      notificationType
    );
  }
}
