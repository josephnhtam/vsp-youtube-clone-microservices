import {ComponentFixture, TestBed} from '@angular/core/testing';

import {PlaylistHeaderComponent} from './playlist-header.component';

describe('PlaylistHeaderComponent', () => {
  let component: PlaylistHeaderComponent;
  let fixture: ComponentFixture<PlaylistHeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PlaylistHeaderComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PlaylistHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
