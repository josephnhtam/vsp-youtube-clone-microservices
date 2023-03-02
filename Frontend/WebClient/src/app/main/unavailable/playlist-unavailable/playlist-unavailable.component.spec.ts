import {ComponentFixture, TestBed} from '@angular/core/testing';

import {PlaylistUnavailableComponent} from './playlist-unavailable.component';

describe('PlaylistUnavailableComponent', () => {
  let component: PlaylistUnavailableComponent;
  let fixture: ComponentFixture<PlaylistUnavailableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PlaylistUnavailableComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PlaylistUnavailableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
