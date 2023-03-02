import {ComponentFixture, TestBed} from '@angular/core/testing';

import {EditVideoDialogComponent} from './edit-video-dialog.component';

describe('EditVideoDialogComponent', () => {
  let component: EditVideoDialogComponent;
  let fixture: ComponentFixture<EditVideoDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditVideoDialogComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditVideoDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
