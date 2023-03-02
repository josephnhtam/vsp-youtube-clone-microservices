import {ComponentFixture, TestBed} from '@angular/core/testing';

import {UploaderDialogComponent} from './uploader-dialog.component';

describe('UploaderDialogComponent', () => {
  let component: UploaderDialogComponent;
  let fixture: ComponentFixture<UploaderDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UploaderDialogComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UploaderDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
