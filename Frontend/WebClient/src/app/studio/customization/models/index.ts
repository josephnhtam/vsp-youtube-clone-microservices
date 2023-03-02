import {ChannelSection} from 'src/app/core/models/channel';

export interface DetailedUserChannel {
  bannerUrl: string | null;
  unsubscribedSpotlightVideoId: string | null;
  subscribedSpotlightVideoId: string | null;
  sections: ChannelSection[];
}

export interface DetailedUserProfile {
  displayName: string;
  description: string;
  handle: string | null;
  email: string | null;
  thumbnailUrl: string | null;
}

export interface UserData {
  userProfile: DetailedUserProfile;
  userChannel: DetailedUserChannel;
}

export interface UpdateUserProfileRequest {
  updateBasicInfo: UpdateUserProfielBasicInfo | null;
  updateImages: UpdateUserProfileImages | null;
  updateLayout: UpdateLayout | null;
}

export interface UpdateUserProfielBasicInfo {
  displayName: string;
  description: string;
  handle: string | null;
  email: string | null;
}

export interface UpdateUserProfileImages {
  thumbnailChanged: boolean;
  bannerChanged: boolean;
  newThubmnailToken: string | null;
  newBannerToken: string | null;
}

export interface UpdateLayout {
  channelSections: ChannelSection[];
}

export interface BasicInfoUpdate {
  displayName: string;
  description: string;
  handle: string;
  email: string;
}

export interface BrandingUpdate {
  thumbnailChanged: boolean;
  bannerChanged: boolean;
  newThubmnailFile: File | null;
  newBannerFile: File | null;
}

export interface LayoutUpdate {
  unsubscribedSpotlightVideoId: string | null;
  subscribedSpotlightVideoId: string | null;
  channelSections: ChannelSection[];
}
