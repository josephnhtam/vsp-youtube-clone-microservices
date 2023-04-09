import { AppSetup } from 'src/app/app-setup';
import { AssetSetup } from 'src/app/asset-setup';
import { AuthSetup } from '../app/auth/auth-setup';
import { StudioSetup } from '../app/studio/studio-setup';

const authSetup: AuthSetup = {
  idpUrl: 'https://auth.vspsample.online',
  clientId: 'spa',
  scope: 'openid email profile roles vsp_api offline_access',
  refreshTokenLifetime: 2592000,
  autoRefreshToken: true,
};

const appSetup: AppSetup = {
  apiUrl: '',
  storageUrl: 'https://storage.vspsample.online',
};

const studioSetup: StudioSetup = {
  videoRefreshIntervalSeconds: 5,
  maxSdResolution: 480,
};

const assetSetup: AssetSetup = {
  noThumbnailIconUrl: 'assets/icon/no_thumbnail.png',
};

export const environment = {
  production: false,
  appSetup,
  authSetup,
  studioSetup,
  assetSetup,
};
