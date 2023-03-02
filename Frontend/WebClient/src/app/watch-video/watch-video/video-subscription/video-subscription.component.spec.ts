import {ComponentFixture, TestBed} from '@angular/core/testing';

import {VideoSubscriptionComponent} from './video-subscription.component';

describe('VideoSubscriptionComponent', () => {
  let component: VideoSubscriptionComponent;
  let fixture: ComponentFixture<VideoSubscriptionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ VideoSubscriptionComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(VideoSubscriptionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
