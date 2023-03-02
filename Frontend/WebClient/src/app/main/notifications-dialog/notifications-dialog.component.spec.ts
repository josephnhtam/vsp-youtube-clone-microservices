import {ComponentFixture, TestBed} from '@angular/core/testing';

import {NotificationsDialogComponent} from './notifications-dialog.component';

describe('NotificationsDialogComponent', () => {
  let component: NotificationsDialogComponent;
  let fixture: ComponentFixture<NotificationsDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ NotificationsDialogComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NotificationsDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
