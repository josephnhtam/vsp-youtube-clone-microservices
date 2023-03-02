import {TestBed} from '@angular/core/testing';

import {UserReadyGuard} from './user-ready-guard.service';

describe('UserReadyGuardService', () => {
  let service: UserReadyGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(UserReadyGuard);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
