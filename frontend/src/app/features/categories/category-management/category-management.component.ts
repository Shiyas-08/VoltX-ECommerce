import { Component } from '@angular/core';
import { CategoryService } from 'src/app/core/services/category.service';
import { OnInit } from '@angular/core';

@Component({
  selector: 'app-category-management',
  templateUrl: './category-management.component.html',
  styleUrls: ['./category-management.component.css']
})
export class CategoryManagementComponent implements OnInit {
  categories: any[] = [];

  name = '';
  editId: number | null = null;

  constructor(private categoryService: CategoryService) {}

  ngOnInit() {
    this.loadCategories();
  }

  loadCategories() {
    this.categoryService.getAll().subscribe(res => {
      this.categories = res.data;
    });
  }

  submit() {
    if (!this.name.trim()) return;

    if (this.editId) {
      this.categoryService.update(this.editId, this.name)
        .subscribe(() => {
          this.reset();
          this.loadCategories();
        });
    } else {
      this.categoryService.create(this.name)
        .subscribe(() => {
          this.reset();
          this.loadCategories();
        });
    }
  }

  edit(category: any) {
    this.editId = category.id;
    this.name = category.name;
  }

  toggle(id: number) {
    this.categoryService.toggle(id)
      .subscribe(() => this.loadCategories());
  }

  reset() {
    this.name = '';
    this.editId = null;
  }
}

