import {ComponentFixture, TestBed} from '@angular/core/testing';

import {PlaylistGridSelectorComponent} from './playlist-grid-selector.component';

describe('PlaylistGridSelectorComponent', () => {
  let component: PlaylistGridSelectorComponent;
  let fixture: ComponentFixture<PlaylistGridSelectorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PlaylistGridSelectorComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PlaylistGridSelectorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
