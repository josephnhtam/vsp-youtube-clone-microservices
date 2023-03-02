import {ComponentFixture, TestBed} from '@angular/core/testing';

import {ChannelUnavailableComponent} from './channel-unavailable.component';

describe('ChannelUnavailableComponent', () => {
  let component: ChannelUnavailableComponent;
  let fixture: ComponentFixture<ChannelUnavailableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChannelUnavailableComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChannelUnavailableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
