import {ComponentFixture, TestBed} from '@angular/core/testing';

import {ChannelSectionRowComponent} from './channel-section-row.component';

describe('ChannelSectionRowComponent', () => {
  let component: ChannelSectionRowComponent;
  let fixture: ComponentFixture<ChannelSectionRowComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChannelSectionRowComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChannelSectionRowComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
