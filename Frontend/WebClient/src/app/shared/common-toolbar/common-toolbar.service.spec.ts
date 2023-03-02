import {TestBed} from '@angular/core/testing';

import {CommonToolbarService} from './common-toolbar.service';

describe('CommonToolbarService', () => {
  let service: CommonToolbarService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CommonToolbarService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
