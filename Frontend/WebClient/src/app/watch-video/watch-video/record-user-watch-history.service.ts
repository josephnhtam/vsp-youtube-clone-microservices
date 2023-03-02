import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';

@Injectable()
export class RecordUserWatchHistoryService {
  constructor(private httpClient: HttpClient) {}

  recordWatch(videoId: string) {
    const url = environment.appSetup.apiUrl + '/api/v1/UserHistory/Record';

    const params = new HttpParams({
      fromObject: {
        videoId,
      },
    });

    return this.httpClient.post(url, null, {
      params,
    });
  }
}
