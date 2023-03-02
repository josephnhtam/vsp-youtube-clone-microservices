import { MatMenuTrigger } from '@angular/material/menu';
import { Store } from '@ngrx/store';
import {
  MessageType,
  NotificationMessage,
} from '../../../core/models/notification';
import { Component, Input, OnInit } from '@angular/core';
import { UserThunmbnailData } from 'src/app/shared/user-thumbnail/models';
import { environment } from 'src/environments/environment';
import * as moment from 'moment';
import { NotificationAction } from '../../../core/actions';
import { NotificationType } from '../../../core/models/subscription';
import { ChannelSubscriptionService } from 'src/app/core/services/channel-subscription.service';

@Component({
  selector: 'app-notification-message',
  templateUrl: './notification-message.component.html',
  styleUrls: ['./notification-message.component.css'],
})
export class NotificationMessageComponent implements OnInit {
  @Input() message!: NotificationMessage;
  @Input() menuTrigger?: MatMenuTrigger;

  focusing = false;

  constructor(
    private store: Store,
    private service: ChannelSubscriptionService
  ) {}

  ngOnInit(): void {}

  get senderThumbnailData() {
    const thumbnailData: UserThunmbnailData = {
      id: this.message.sender.userId ?? '',
      handle: null,
      displayName: this.message.sender.displayName,
      thumbnailUrl: this.message.sender.thumbnailUrl,
    };

    return thumbnailData;
  }

  get thumbnailUrl() {
    return (
      this.message.thumbnailUrl || environment.assetSetup.noThumbnailIconUrl
    );
  }

  get senderDisplayName() {
    return this.message.sender.displayName;
  }

  get contentMessage() {
    if (this.message.type == MessageType.VideoUploaded) {
      return `${this.message.sender.displayName} uploaded: ${this.message.content}`;
    }

    return this.message.content;
  }

  get date() {
    return moment(this.message.date).fromNow();
  }

  get checked() {
    return this.message.checked;
  }

  get viewLink() {
    if (this.message.type == MessageType.VideoUploaded) {
      return ['/watch', this.message.id];
    }

    return null;
  }

  failedToLoadThumbnail() {
    this.message = {
      ...this.message,
      thumbnailUrl: null,
    };
  }

  hideMessage() {
    this.store.dispatch(
      NotificationAction.removeMessage({ messageId: this.message.id })
    );
  }

  turnOffNotification() {
    if (!this.message.sender.userId) return;

    this.service.changeNotificationType(
      this.message.sender.userId,
      NotificationType.None
    );
  }

  onClick() {
    if (!this.message.checked) {
      this.store.dispatch(
        NotificationAction.markMessageAsChecked({
          messageId: this.message.id,
        })
      );
    }

    this.menuTrigger?.closeMenu();
  }
}
