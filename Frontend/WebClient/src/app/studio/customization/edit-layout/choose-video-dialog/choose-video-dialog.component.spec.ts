import {ComponentFixture, TestBed} from '@angular/core/testing';

import {ChooseVideoDialogComponent} from './choose-video-dialog.component';

describe('ChooseVideoDialogComponent', () => {
  let component: ChooseVideoDialogComponent;
  let fixture: ComponentFixture<ChooseVideoDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChooseVideoDialogComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChooseVideoDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
