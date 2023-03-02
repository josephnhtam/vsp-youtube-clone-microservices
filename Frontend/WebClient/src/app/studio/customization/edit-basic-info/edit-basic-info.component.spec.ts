import {ComponentFixture, TestBed} from '@angular/core/testing';

import {EditBasicInfoComponent} from './edit-basic-info.component';

describe('EditBasicInfoComponent', () => {
  let component: EditBasicInfoComponent;
  let fixture: ComponentFixture<EditBasicInfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditBasicInfoComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditBasicInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
