import {ComponentFixture, TestBed} from '@angular/core/testing';

import {UserWatchHistoryComponent} from './user-watch-history.component';

describe('UserWatchHistoryComponent', () => {
  let component: UserWatchHistoryComponent;
  let fixture: ComponentFixture<UserWatchHistoryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UserWatchHistoryComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserWatchHistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
