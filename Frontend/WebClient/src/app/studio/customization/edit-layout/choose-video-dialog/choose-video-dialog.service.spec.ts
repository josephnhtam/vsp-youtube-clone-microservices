import {TestBed} from '@angular/core/testing';

import {ChooseVideoDialogService} from './choose-video-dialog.service';

describe('ChooseVideoDialogService', () => {
  let service: ChooseVideoDialogService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ChooseVideoDialogService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
