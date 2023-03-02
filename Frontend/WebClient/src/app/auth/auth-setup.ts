export interface AuthSetup {
  idpUrl: string;
  clientId: string;
  scope: string;
  refreshTokenLifetime: number;
  autoRefreshToken: boolean;
}
