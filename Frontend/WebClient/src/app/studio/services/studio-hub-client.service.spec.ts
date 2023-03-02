import {TestBed} from '@angular/core/testing';

import {StudioHubClientService} from './studio-hub-client.service';

describe('StudioSignalrService', () => {
  let service: StudioHubClientService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StudioHubClientService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
