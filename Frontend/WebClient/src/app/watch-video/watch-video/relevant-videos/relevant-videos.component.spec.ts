import {ComponentFixture, TestBed} from '@angular/core/testing';

import {RelevantVideosComponent} from './relevant-videos.component';

describe('RelevantVideosComponent', () => {
  let component: RelevantVideosComponent;
  let fixture: ComponentFixture<RelevantVideosComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RelevantVideosComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RelevantVideosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
