import {ComponentFixture, TestBed} from '@angular/core/testing';

import {ChannelFeaturedComponent} from './channel-featured.component';

describe('ChannelFeaturedComponent', () => {
  let component: ChannelFeaturedComponent;
  let fixture: ComponentFixture<ChannelFeaturedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChannelFeaturedComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChannelFeaturedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
