import {ComponentFixture, TestBed} from '@angular/core/testing';

import {EditBrandingComponent} from './edit-branding.component';

describe('EditBrandingComponent', () => {
  let component: EditBrandingComponent;
  let fixture: ComponentFixture<EditBrandingComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditBrandingComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditBrandingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
