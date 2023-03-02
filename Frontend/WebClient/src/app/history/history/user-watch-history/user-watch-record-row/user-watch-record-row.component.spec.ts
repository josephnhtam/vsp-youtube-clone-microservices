import {ComponentFixture, TestBed} from '@angular/core/testing';

import {UserWatchRecordRowComponent} from './user-watch-record-row.component';

describe('UserWatchRecordRowComponent', () => {
  let component: UserWatchRecordRowComponent;
  let fixture: ComponentFixture<UserWatchRecordRowComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UserWatchRecordRowComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserWatchRecordRowComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
