import { catchError, of } from 'rxjs';
import { environment } from './../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable()
export class NotificationToolService {
  constructor(private httpClient: HttpClient) {}

  getUnreadNotificationMessageCount() {
    const url =
      environment.appSetup.apiUrl + '/api/v1/Notifications/UnreadCount';

    return this.httpClient.get<number | null>(url).pipe(
      catchError((error) => {
        console.error(error);
        return of(null);
      })
    );
  }

  resetUnreadNotificationMessageCount() {
    const url =
      environment.appSetup.apiUrl + '/api/v1/Notifications/UnreadCount';

    return this.httpClient.put(url, null);
  }
}
