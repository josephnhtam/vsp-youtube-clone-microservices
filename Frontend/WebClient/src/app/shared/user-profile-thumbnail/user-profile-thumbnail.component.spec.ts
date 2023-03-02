import {ComponentFixture, TestBed} from '@angular/core/testing';

import {UserProfileThumbnailComponent} from './user-profile-thumbnail.component';

describe('UserProfileThumbnailComponent', () => {
  let component: UserProfileThumbnailComponent;
  let fixture: ComponentFixture<UserProfileThumbnailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UserProfileThumbnailComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserProfileThumbnailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
