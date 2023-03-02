import {TestBed} from '@angular/core/testing';

import {AuthInfoResolver} from './auth-info-resolver.service';

describe('AuthInfoResolver', () => {
  let resolver: AuthInfoResolver;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    resolver = TestBed.inject(AuthInfoResolver);
  });

  it('should be created', () => {
    expect(resolver).toBeTruthy();
  });
});
