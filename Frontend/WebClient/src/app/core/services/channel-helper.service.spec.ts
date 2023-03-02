import {TestBed} from '@angular/core/testing';

import {ChannelHelperService} from './channel-helper.service';

describe('ChannelSectionService', () => {
  let service: ChannelHelperService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ChannelHelperService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
