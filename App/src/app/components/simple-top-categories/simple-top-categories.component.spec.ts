import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SimpleTopCategoriesComponent } from './simple-top-categories.component';

describe('SimpleTopCategoriesComponent', () => {
  let component: SimpleTopCategoriesComponent;
  let fixture: ComponentFixture<SimpleTopCategoriesComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [SimpleTopCategoriesComponent]
    });
    fixture = TestBed.createComponent(SimpleTopCategoriesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
