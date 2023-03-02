import {ComponentFixture, TestBed} from '@angular/core/testing';

import {SubscriptionsFeedComponent} from './subscriptions-feed.component';

describe('SubscriptionsFeedComponent', () => {
  let component: SubscriptionsFeedComponent;
  let fixture: ComponentFixture<SubscriptionsFeedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SubscriptionsFeedComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SubscriptionsFeedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
