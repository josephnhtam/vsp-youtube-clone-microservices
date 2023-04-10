import { AssetSetup } from '../app/asset-setup';
import { AuthSetup } from '../app/auth/auth-setup';
import { StudioSetup } from '../app/studio/studio-setup';
import { AppSetup } from 'src/app/app-setup';

// const authSetup: AuthSetup = {
//   idpUrl: 'https://auth.vspsample.online',
//   clientId: 'spa',
//   scope: 'openid email profile roles vsp_api offline_access',
//   refreshTokenLifetime: 2592000,
//   autoRefreshToken: true,
// };

// const appSetup: AppSetup = {
//   apiUrl: 'https://vspsample.online',
//   storageUrl: 'https://storage.vspsample.online',
// };

const authSetup: AuthSetup = {
  idpUrl: 'http://localhost:15100',
  clientId: 'spa',
  scope: 'openid email profile roles vsp_api offline_access',
  refreshTokenLifetime: 2592000,
  autoRefreshToken: true,
};

const appSetup: AppSetup = {
  apiUrl: 'http://localhost:16000',
  storageUrl: 'http://localhost:14200',
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
  appSetup: appSetup,
  authSetup,
  studioSetup,
  assetSetup,
};
