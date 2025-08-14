import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CategoryMenuComponent } from './category-menu.component';

describe('CategoryMenuComponent', () => {
  let component: CategoryMenuComponent;
  let fixture: ComponentFixture<CategoryMenuComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [CategoryMenuComponent]
    });
    fixture = TestBed.createComponent(CategoryMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
