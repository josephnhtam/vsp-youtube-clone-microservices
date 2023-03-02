import {TestBed} from '@angular/core/testing';

import {HistoryGuard} from './history-guard.service';

describe('HistoryGuardService', () => {
  let service: HistoryGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(HistoryGuard);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
