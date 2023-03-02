import {ComponentFixture, TestBed} from '@angular/core/testing';
import {EditVideoVisibilityComponent} from './edit-video-visibility.component';

describe('EditVideoVisibilityComponent', () => {
  let component: EditVideoVisibilityComponent;
  let fixture: ComponentFixture<EditVideoVisibilityComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [EditVideoVisibilityComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(EditVideoVisibilityComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
