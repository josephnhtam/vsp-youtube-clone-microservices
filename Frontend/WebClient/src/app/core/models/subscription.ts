export enum NotificationType {
  None = 0,
  Normal = 1,
  Enhanced = 2,
}

export interface SubscriptionStatus {
  subscribersCount: number;
  isSubscribed: boolean;
  notificationType: NotificationType;
}

export interface SubscriptionTargetIds {
  subscriptionTargetIds: string[];
}

export interface UserSubscriptionInfo {
  subscribersCount: number;
  subscriptionsCount: number;
}

export interface DetailedUserProfile {
  id: string;
  displayName: string;
  description: string;
  handle: string | null;
  thumbnailUrl: string | null;
  subscribersCount: number;
  createDate: string;
}

export interface DetailedSubscription {
  userProfile: DetailedUserProfile;
  notificationType: NotificationType;
  subscriptionDate: string;
}

export interface SubscriptionClient {
  userProfile: DetailedUserProfile;
  notificationType: NotificationType;
  subscriptionDate: string;
  isSubscribed: boolean;
}

export interface UserProfile {
  id: string;
  handle: string | null;
  displayName: string;
  thumbnailUrl: string | null;
}

export enum SubscriptionTargetSort {
  DisplayName,
  SubscriptionDate,
}

export interface DetailedSubscriptions {
  totalCount: number;
  subscriptions: DetailedSubscription[];
}

export interface Subscription {
  userProfile: UserProfile;
  subscriptionDate: string;
  parsedSubscriptionDate: Date;
}

export interface Subscriptions {
  totalCount: number;
  subscriptions: Subscription[];
}

export interface UserProfile {
  id: string;
  handle: string | null;
  displayName: string;
  thumbnailUrl: string | null;
}

export interface SubscriptionStatus {
  subscribersCount: number;
  isSubscribed: boolean;
  notificationType: NotificationType;
}

export interface ChangeNotificationTypeRequest {
  userId: string;
  notificationType: NotificationType;
}

export interface SubscribeRequest {
  userId: string;
  notificationType: NotificationType;
}
