import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BrandNewComponent } from './brand-new.component';

describe('BrandNewComponent', () => {
  let component: BrandNewComponent;
  let fixture: ComponentFixture<BrandNewComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [BrandNewComponent]
    });
    fixture = TestBed.createComponent(BrandNewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
