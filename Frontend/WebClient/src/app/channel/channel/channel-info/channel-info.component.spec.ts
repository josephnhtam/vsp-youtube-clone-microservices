import {ComponentFixture, TestBed} from '@angular/core/testing';

import {ChannelInfoComponent} from './channel-info.component';

describe('ChannelInfoComponent', () => {
  let component: ChannelInfoComponent;
  let fixture: ComponentFixture<ChannelInfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChannelInfoComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChannelInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
