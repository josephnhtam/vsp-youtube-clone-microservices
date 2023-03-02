import {TestBed} from '@angular/core/testing';

import {FailedResponseInterceptor} from './failed-response-interceptor.service';

describe('FailedResponseInterceptorService', () => {
  let service: FailedResponseInterceptor;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(FailedResponseInterceptor);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
