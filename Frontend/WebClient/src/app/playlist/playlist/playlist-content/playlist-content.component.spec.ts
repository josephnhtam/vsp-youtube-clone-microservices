import {ComponentFixture, TestBed} from '@angular/core/testing';

import {PlaylistContentComponent} from './playlist-content.component';

describe('PlaylistContentComponent', () => {
  let component: PlaylistContentComponent;
  let fixture: ComponentFixture<PlaylistContentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PlaylistContentComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PlaylistContentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
