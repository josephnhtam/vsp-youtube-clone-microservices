export interface FailedResponse {
  statusCode: number;
  message: string;
  errors: { [key: string]: string } | null;
}
