import {ComponentFixture, TestBed} from '@angular/core/testing';

import {ChannelSectionComponent} from './channel-section.component';

describe('ChannelSectionComponent', () => {
  let component: ChannelSectionComponent;
  let fixture: ComponentFixture<ChannelSectionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChannelSectionComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChannelSectionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
