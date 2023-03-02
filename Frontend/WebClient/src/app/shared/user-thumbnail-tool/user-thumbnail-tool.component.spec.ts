import {ComponentFixture, TestBed} from '@angular/core/testing';

import {UserThumbnailToolComponent} from './user-thumbnail-tool.component';

describe('UserThumbnailToolComponent', () => {
  let component: UserThumbnailToolComponent;
  let fixture: ComponentFixture<UserThumbnailToolComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UserThumbnailToolComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserThumbnailToolComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
