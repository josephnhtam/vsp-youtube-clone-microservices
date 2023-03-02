import {ComponentFixture, TestBed} from '@angular/core/testing';

import {EditLayoutComponent} from './edit-layout.component';

describe('EditLayoutComponent', () => {
  let component: EditLayoutComponent;
  let fixture: ComponentFixture<EditLayoutComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditLayoutComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditLayoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
