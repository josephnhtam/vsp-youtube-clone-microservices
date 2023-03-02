import {ComponentFixture, TestBed} from '@angular/core/testing';
import {EditVideoDetailsComponent} from './edit-video-details.component';

describe('EditVideoDetailsComponent', () => {
  let component: EditVideoDetailsComponent;
  let fixture: ComponentFixture<EditVideoDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [EditVideoDetailsComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(EditVideoDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
