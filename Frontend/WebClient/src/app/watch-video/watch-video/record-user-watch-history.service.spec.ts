import {TestBed} from '@angular/core/testing';

import {RecordUserWatchHistoryService} from './record-user-watch-history.service';

describe('RecordUserWatchHistoryService', () => {
  let service: RecordUserWatchHistoryService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(RecordUserWatchHistoryService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
