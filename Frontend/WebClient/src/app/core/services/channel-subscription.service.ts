import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { SubscriptionManagementAction } from 'src/app/core/actions';
import { environment } from 'src/environments/environment';
import {
  NotificationType,
  SubscriptionStatus,
  UserProfile,
} from '../../core/models/subscription';

@Injectable()
export class ChannelSubscriptionService {
  constructor(private httpClient: HttpClient, private store: Store) {}

  getSubscriptionStatus(
    subscriptionTargetId?: string,
    subscriptionTargetHandle?: string
  ): Observable<SubscriptionStatus> {
    const url = environment.appSetup.apiUrl + '/api/v1/Subscriptions/Status';

    let params = new HttpParams();

    if (!!subscriptionTargetId) {
      params = params.append('subscriptionTargetId', subscriptionTargetId);
    }

    if (!!subscriptionTargetHandle) {
      params = params.append(
        'subscriptionTargetHandle',
        subscriptionTargetHandle
      );
    }

    return this.httpClient.get<SubscriptionStatus>(url, {
      params,
    });
  }

  subscribe(userProfile: UserProfile, notificationType: NotificationType) {
    this.store.dispatch(
      SubscriptionManagementAction.subscribe({
        userId: userProfile.id,
        notificationType,
        userProfile,
      })
    );
  }

  unsubscribe(userId: string) {
    this.store.dispatch(SubscriptionManagementAction.unsubscribe({ userId }));
  }

  changeNotificationType(userId: string, notificationType: NotificationType) {
    this.store.dispatch(
      SubscriptionManagementAction.changeNotificationType({
        userId,
        notificationType,
      })
    );
  }
}
