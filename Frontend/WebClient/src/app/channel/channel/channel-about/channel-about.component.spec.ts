import {ComponentFixture, TestBed} from '@angular/core/testing';

import {ChannelAboutComponent} from './channel-about.component';

describe('ChannelAboutComponent', () => {
  let component: ChannelAboutComponent;
  let fixture: ComponentFixture<ChannelAboutComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChannelAboutComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChannelAboutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
