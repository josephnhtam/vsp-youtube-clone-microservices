export interface UserProfile {
  id: string;
  handle: string | null;
  displayName: string;
  thumbnailUrl: string | null;
  status?: UserProfileStatus;
}

export interface UserProfileClient {
  id: string;
  userProfile: UserProfile | null;
  pending: boolean;
  error: any | null;
}

export enum UserProfileStatus {
  Created = 0,
  Registered = 1,
  RegistrationFailed = 2,
}
