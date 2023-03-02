import {Component, Input} from '@angular/core';
import {getChannelLink} from '../../core/services/utilities';
import {UserThunmbnailData} from './models';

@Component({
  selector: 'app-user-thumbnail',
  templateUrl: './user-thumbnail.component.html',
  styleUrls: ['./user-thumbnail.component.css'],
})
export class UserThumbnailComponent {
  @Input() userThumbnailData?: UserThunmbnailData;
  @Input() hasHyperlink = true;
  @Input() size = 40;

  get userDisplayNameLetter() {
    if (
      !this.userThumbnailData ||
      this.userThumbnailData.displayName.trim().length == 0
    )
      return '';
    return this.userThumbnailData.displayName.trim()[0];
  }

  get sizeStyle() {
    return {
      width: `${this.size}px`,
      height: `${this.size}px`,
      fontSize: `${this.size * 0.45}px`,
    };
  }

  get channelLink() {
    return getChannelLink(this.userThumbnailData);
  }

  failedToLoadThumbnail() {
    if (this.userThumbnailData) {
      this.userThumbnailData.thumbnailUrl = null;
    }
  }
}
