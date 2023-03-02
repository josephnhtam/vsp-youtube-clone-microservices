import {TestBed} from '@angular/core/testing';

import {NotificationToolService} from './notification-tool.service';

describe('NotificationToolService', () => {
  let service: NotificationToolService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(NotificationToolService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
