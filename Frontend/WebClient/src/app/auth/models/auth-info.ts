import {TokenResponseJson} from '@openid/appauth';

export interface AuthInfo {
  sub: string;
  email: string;
  name: string;
  role: string[];

  token: TokenResponseJson;
}
