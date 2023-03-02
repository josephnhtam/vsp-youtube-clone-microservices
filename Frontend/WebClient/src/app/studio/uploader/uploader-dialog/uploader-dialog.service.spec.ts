import {TestBed} from '@angular/core/testing';

import {UploaderDialogService} from './uploader-dialog.service';

describe('UploaderDialogService', () => {
  let service: UploaderDialogService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(UploaderDialogService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
