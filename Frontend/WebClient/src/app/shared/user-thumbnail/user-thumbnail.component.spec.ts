import {ComponentFixture, TestBed} from '@angular/core/testing';

import {UserThumbnailComponent} from './user-thumbnail.component';

describe('UserThumbnailComponent', () => {
  let component: UserThumbnailComponent;
  let fixture: ComponentFixture<UserThumbnailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UserThumbnailComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserThumbnailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
