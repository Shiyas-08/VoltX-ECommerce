import { Component, OnInit } from '@angular/core';
import { ProductService } from 'src/app/core/services/product.service';
import { CategoryService } from 'src/app/core/services/category.service';
import { ToastrService } from 'ngx-toastr';
import { AdminProduct } from 'src/app/core/models/product.model';

@Component({
  selector: 'app-product-management',
  templateUrl: './product-management.component.html',
})
export class ProductManagementComponent implements OnInit {

  products: AdminProduct[] = [];
  categories: any[] = [];

  showAddModal = false;
  showEditModal = false;

  productForm = {
    id: null as number | null,
    name: '',
    price: null as number | null,
    stock: null as number | null,
    categoryId: null as number | null,
    description: '',
    isFeatured: false
  };

  productImages: { id: number; imageUrl: string }[] = [];
  selectedImages: File[] = [];

  constructor(
    private productService: ProductService,
    private categoryService: CategoryService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.loadProducts();
    this.loadCategories();
  }


  loadProducts() {
    this.productService.getAdminProducts().subscribe({
      next: res => this.products = res.data,
      error: () => this.toastr.error('Failed to load products')
    });
  }

  loadCategories() {
    this.categoryService.getAll().subscribe(res => {
      this.categories = res.data.filter((c: any) => c.isActive);
    });
  }

//models

  openAddModal() {
    this.resetForm();
    this.showAddModal = true;
  }

  openEditModal(p: AdminProduct) {
    this.productForm = {
      id: p.id,
      name: p.name,
      price: p.price,
      stock: p.stock,
      categoryId: p.categoryId,
      description: p.description,
      isFeatured: p.isFeatured
    };
//image management
    this.productImages = p.images ? [...p.images] : [];
    this.selectedImages = [];
    this.showEditModal = true;
  }

  closeModal() {
    this.showAddModal = false;
    this.showEditModal = false;
    this.productImages = [];
    this.selectedImages = [];
  }

  resetForm() {
    this.productForm = {
      id: null,
      name: '',
      price: null,
      stock: null,
      categoryId: null,
      description: '',
      isFeatured: false
    };
    this.productImages = [];
    this.selectedImages = [];
  }

  

  onImageSelect(event: any) {
    this.selectedImages = Array.from(event.target.files);
  }



  buildFormData(): FormData {
    const fd = new FormData();

    fd.append('Name', this.productForm.name);
    fd.append('Price', String(this.productForm.price ?? 0));
    fd.append('Stock', String(this.productForm.stock ?? 0));
    fd.append('CategoryId', String(this.productForm.categoryId ?? 0));
    fd.append('Description', this.productForm.description || '');
    fd.append('IsFeatured', String(this.productForm.isFeatured));

    this.selectedImages.forEach(img => {
      fd.append('Images', img);
    });

    return fd;
  }

  //crud
saveProduct() {
  this.productService.createProduct(this.buildFormData()).subscribe({
    next: () => {
      this.toastr.success('Product created successfully');
      this.closeModal();
      this.loadProducts();
    },
    error: err => {
      const message =
        err?.error?.message ||
        err?.error?.Message ||
        err?.error?.data ||
        'Create product failed';

      this.toastr.error(message);
    }
  });
}


  updateProduct() {
  if (!this.productForm.id) return;

  this.productService
    .updateProduct(this.productForm.id, this.buildFormData())
    .subscribe({
      next: () => {
        this.toastr.success('Product updated successfully');
        this.closeModal();
        this.loadProducts();
      },
      error: err => {
        const message =
          err?.error?.message ||
          err?.error?.Message ||
          err?.error?.data ||
          'Update failed';

        this.toastr.error(message);
      }
    });
}


  toggleProduct(id: number) {
    this.productService.toggleProduct(id).subscribe({
      next: () => {
        this.toastr.success('Product status updated');
        this.loadProducts();
      },
      error: () => this.toastr.error('Toggle failed')
    });
  }

  deleteProduct(id: number) {
    if (!confirm('Deactivate product?')) return;

    this.productService.deleteProduct(id).subscribe({
      next: () => {
        this.toastr.success('Product deactivated');
        this.loadProducts();
      },
      error: () => this.toastr.error('Delete failed')
    });
  }


  uploadImage(event: any) {
    if (!this.productForm.id) return;

    const file = event.target.files[0];
    if (!file) return;

    this.productService
      .addProductImage(this.productForm.id, file)
      .subscribe({
        next: res => {
          this.toastr.success('Image added successfully');

          
          this.productImages.push({
            id: Date.now(), 
            imageUrl: res.data
          });
        },
        error: () => this.toastr.error('Image upload failed')
      });
  }

  removeImage(imageId: number) {
    if (!confirm('Remove image?')) return;

    this.productService.removeProductImage(imageId).subscribe({
      next: () => {
        this.productImages =
          this.productImages.filter(i => i.id !== imageId);
        this.toastr.success('Image removed successfully');
      },
      error: () => this.toastr.error('Failed to remove image')
    });
  }
}
