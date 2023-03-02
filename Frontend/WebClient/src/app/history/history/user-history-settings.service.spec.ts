import {TestBed} from '@angular/core/testing';

import {UserHistorySettingsService} from './user-history-settings.service';

describe('UserHistorySettingsService', () => {
  let service: UserHistorySettingsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(UserHistorySettingsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
