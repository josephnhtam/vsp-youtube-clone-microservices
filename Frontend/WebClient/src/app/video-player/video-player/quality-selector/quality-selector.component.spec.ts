import {ComponentFixture, TestBed} from '@angular/core/testing';

import {QualitySelectorComponent} from './quality-selector.component';

describe('QualitySelectorComponent', () => {
  let component: QualitySelectorComponent;
  let fixture: ComponentFixture<QualitySelectorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ QualitySelectorComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(QualitySelectorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
