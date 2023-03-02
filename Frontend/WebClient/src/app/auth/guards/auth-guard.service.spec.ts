import {TestBed} from '@angular/core/testing';

import {AuthGuard} from './auth-guard.service';

describe('AuthGuardGuard', () => {
  let guard: AuthGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(AuthGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
