export interface NotificationMessage {
  id: string;
  type: MessageType;
  sender: NotificationMessageSender;
  thumbnailUrl: string | null;
  content: string;
  date: string;
  checked: boolean;
}

export interface NotificationMessageSender {
  userId: string | null;
  displayName: string;
  thumbnailUrl: string | null;
}

export enum MessageType {
  VideoUploaded,
}

export interface GetNotificationMessageResponse {
  totalCount: number;
  messages: NotificationMessage[];
}
