import {ComponentFixture, TestBed} from '@angular/core/testing';

import {UploadProcessRowComponent} from './upload-process-row.component';

describe('UploadProcessRowComponent', () => {
  let component: UploadProcessRowComponent;
  let fixture: ComponentFixture<UploadProcessRowComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UploadProcessRowComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UploadProcessRowComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
