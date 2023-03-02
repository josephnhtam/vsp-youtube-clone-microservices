export interface UserData {
  id: string;
  handle: string | null;
}

export function getChannelLink(userData?: UserData) {
  if (userData) {
    if (!!userData.handle) {
      return ['/@' + userData.handle];
    } else {
      return ['/channel', userData.id];
    }
  } else {
    return [];
  }
}
