import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, takeUntil, forkJoin } from 'rxjs';
import { TranslateModule } from '@ngx-translate/core';

import { AdminService } from '../../services/admin.service';
import { CategoryService } from '../../services/category.service';
import { AdminProduct, SaleToggle, StockUpdate, BulkUpdate } from '../../models/admin.model';
import { Category } from '../../models/category.model';
import { CreateProduct } from '../../models/product.model';

@Component({
  selector: 'app-product-management',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule],
  templateUrl: './product-management.html',
  styleUrl: './product-management.scss'
})
export class ProductManagementComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  products: AdminProduct[] = [];
  filteredProducts: AdminProduct[] = [];
  categories: Category[] = [];
  isLoading = true;

  // Filters
  searchTerm = '';
  selectedCategory = '';
  selectedStatus = '';

  // Selection
  selectedProducts: number[] = [];

  // Debug
  debugInfo = '';

  // Modals
  showProductModal = false;
  showSaleModal = false;
  showBulkModal = false;

  // Forms
  editingProduct: AdminProduct | null = null;
  productForm: any = this.getEmptyProductForm();

  selectedProductForSale: AdminProduct | null = null;
  saleForm = { isOnSale: false, salePrice: 0 };

  bulkAction = '';
  bulkSalePrice = 0;

  constructor(
    private adminService: AdminService,
    private categoryService: CategoryService
  ) { }

  ngOnInit(): void {
    this.loadData();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadData(): void {
    this.isLoading = true;

    forkJoin({
      products: this.adminService.getAllProductsForAdmin(),
      categories: this.categoryService.getAllCategories()
    }).pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.products = data.products;
          this.categories = data.categories;
          this.debugInfo = `Loaded ${data.products.length} products and ${data.categories.length} categories`;
          console.log('Loaded categories:', this.categories); // Debug log
          this.filterProducts();
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Error loading data:', error);
          this.isLoading = false;
        }
      });
  }

  refreshProducts(): void {
    this.loadData();
  }

  filterProducts(): void {
    this.filteredProducts = this.products.filter(product => {
      const matchesSearch = !this.searchTerm ||
        product.name.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        product.brand?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        product.categoryName.toLowerCase().includes(this.searchTerm.toLowerCase());

      const matchesCategory = !this.selectedCategory ||
        product.categoryId.toString() === this.selectedCategory;

      const matchesStatus = !this.selectedStatus || this.getProductStatus(product) === this.selectedStatus;

      return matchesSearch && matchesCategory && matchesStatus;
    });
  }

  private getProductStatus(product: AdminProduct): string {
    if (!product.isActive) return 'inactive';
    if (product.isFeatured) return 'featured';
    if (product.isOnSale) return 'sale';
    if (product.stockQuantity === 0) return 'out-of-stock';
    if (product.stockQuantity < 10) return 'low-stock';
    return 'active';
  }

  // Selection methods
  toggleSelection(productId: number): void {
    const index = this.selectedProducts.indexOf(productId);
    if (index > -1) {
      this.selectedProducts.splice(index, 1);
    } else {
      this.selectedProducts.push(productId);
    }
  }

  isSelected(productId: number): boolean {
    return this.selectedProducts.includes(productId);
  }

  selectAll(): void {
    this.selectedProducts = this.filteredProducts.map(p => p.id);
  }

  clearSelection(): void {
    this.selectedProducts = [];
  }

  isAllSelected(): boolean {
    return this.filteredProducts.length > 0 &&
      this.selectedProducts.length === this.filteredProducts.length;
  }

  toggleSelectAll(): void {
    if (this.isAllSelected()) {
      this.clearSelection();
    } else {
      this.selectAll();
    }
  }

  // Product CRUD operations
  openProductModal(product?: AdminProduct): void {
    console.log('Opening product modal, categories available:', this.categories.length);
    this.editingProduct = product || null;
    this.productForm = product ? { ...product } : this.getEmptyProductForm();
    this.showProductModal = true;
  }

  closeProductModal(): void {
    this.showProductModal = false;
    this.editingProduct = null;
    this.productForm = this.getEmptyProductForm();
  }

  private getEmptyProductForm(): any {
    return {
      name: '',
      description: '',
      price: 0,
      salePrice: undefined,
      stockQuantity: 0,
      imageUrl: '',
      categoryId: '',
      brand: '',
      shade: '',
      size: '',
      ingredients: '',
      skinType: '',
      isActive: true,
      isFeatured: false,
      isOnSale: false
    };
  }

  saveProduct(): void {
    console.log('Saving product with form data:', this.productForm);

    // Validate required fields
    if (!this.productForm.name || !this.productForm.price || !this.productForm.categoryId) {
      alert('Please fill in all required fields (Name, Price, Category)');
      return;
    }

    const productData: CreateProduct = {
      name: this.productForm.name,
      description: this.productForm.description || '',
      price: Number(this.productForm.price),
      salePrice: this.productForm.salePrice ? Number(this.productForm.salePrice) : undefined,
      stockQuantity: Number(this.productForm.stockQuantity) || 0,
      imageUrl: this.productForm.imageUrl || `https://picsum.photos/300/300?random=${Date.now()}`,
      imageUrls: [],
      categoryId: Number(this.productForm.categoryId),
      isFeatured: Boolean(this.productForm.isFeatured),
      isOnSale: Boolean(this.productForm.isOnSale),
      brand: this.productForm.brand || '',
      shade: this.productForm.shade || '',
      size: this.productForm.size || '',
      ingredients: this.productForm.ingredients || '',
      skinType: this.productForm.skinType || ''
    };

    console.log('Processed product data:', productData);

    const operation = this.editingProduct
      ? this.adminService.updateProduct(this.editingProduct.id, {
        ...productData,
        id: this.editingProduct.id,
        isActive: Boolean(this.productForm.isActive)
      })
      : this.adminService.createProduct(productData);

    operation.pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (result) => {
          console.log('Product saved successfully:', result);
          this.debugInfo = `Product ${this.editingProduct ? 'updated' : 'created'} successfully!`;
          this.closeProductModal();
          this.refreshProducts();
        },
        error: (error) => {
          console.error('Error saving product:', error);
          this.debugInfo = `Error saving product: ${error.error?.message || error.message || 'Unknown error'}`;
        }
      });
  }

  editProduct(product: AdminProduct): void {
    this.openProductModal(product);
  }

  deleteProduct(productId: number): void {
    console.log('Delete product clicked:', productId);
    const product = this.products.find(p => p.id === productId);
    const productName = product ? product.name : `Product ${productId}`;

    if (confirm(`Are you sure you want to delete "${productName}"?`)) {
      this.debugInfo = `Deleting product ${productId}...`;

      this.adminService.deleteProduct(productId)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            console.log('Product deleted successfully');
            this.debugInfo = `Product "${productName}" deleted successfully!`;
            this.refreshProducts();
          },
          error: (error) => {
            console.error('Error deleting product:', error);
            this.debugInfo = `Error deleting product: ${error.message || 'Unknown error'}`;
          }
        });
    }
  }

  // Quick actions
  toggleActive(productId: number): void {
    console.log('Toggling active status for product:', productId);
    this.debugInfo = `Toggling active status for product ${productId}...`;

    this.adminService.toggleProductActive(productId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (result) => {
          console.log('Toggle active result:', result);
          this.debugInfo = `Product ${productId} status toggled successfully!`;
          this.refreshProducts();
        },
        error: (error) => {
          console.error('Error toggling active status:', error);
          this.debugInfo = `Error toggling product status: ${error.message || 'Unknown error'}`;
        }
      });
  }

  toggleFeatured(productId: number): void {
    console.log('Toggling featured status for product:', productId);
    this.debugInfo = `Toggling featured status for product ${productId}...`;

    this.adminService.toggleProductFeatured(productId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (result) => {
          console.log('Toggle featured result:', result);
          this.debugInfo = `Product ${productId} featured status toggled successfully!`;
          this.refreshProducts();
        },
        error: (error) => {
          console.error('Error toggling featured status:', error);
          this.debugInfo = `Error toggling featured status: ${error.message || 'Unknown error'}`;
        }
      });
  }

  updateStock(productId: number, event: any): void {
    const quantity = parseInt(event.target.value);
    const stockUpdate: StockUpdate = { quantity };

    this.adminService.updateStock(productId, stockUpdate)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          // Update local data
          const product = this.products.find(p => p.id === productId);
          if (product) {
            product.stockQuantity = quantity;
            product.status = quantity === 0 ? 'Out of Stock' :
              quantity < 10 ? 'Low Stock' : 'In Stock';
          }
          this.filterProducts();
        },
        error: (error) => {
          console.error('Error updating stock:', error);
          event.target.value = event.target.defaultValue; // Reset on error
        }
      });
  }

  // Sale management
  openSaleModal(product: AdminProduct): void {
    this.selectedProductForSale = product;
    this.saleForm = {
      isOnSale: product.isOnSale,
      salePrice: product.salePrice || product.price * 0.8 // Default to 20% off
    };
    this.showSaleModal = true;
  }

  closeSaleModal(): void {
    this.showSaleModal = false;
    this.selectedProductForSale = null;
  }

  applySale(): void {
    if (!this.selectedProductForSale) return;

    const saleData: SaleToggle = {
      isOnSale: this.saleForm.isOnSale,
      salePrice: this.saleForm.isOnSale ? this.saleForm.salePrice : undefined
    };

    this.adminService.toggleProductSale(this.selectedProductForSale.id, saleData)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.closeSaleModal();
          this.refreshProducts();
        },
        error: (error) => {
          console.error('Error applying sale:', error);
        }
      });
  }

  // Bulk operations
  openBulkModal(): void {
    this.bulkAction = '';
    this.bulkSalePrice = 0;
    this.showBulkModal = true;
  }

  closeBulkModal(): void {
    this.showBulkModal = false;
  }

  applyBulkAction(): void {
    if (!this.bulkAction || this.selectedProducts.length === 0) return;

    const bulkUpdate: BulkUpdate = {
      productIds: this.selectedProducts,
      action: this.bulkAction,
      salePrice: this.bulkAction === 'sale' ? this.bulkSalePrice : undefined
    };

    this.adminService.bulkUpdateProducts(bulkUpdate)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.closeBulkModal();
          this.clearSelection();
          this.refreshProducts();
        },
        error: (error) => {
          console.error('Error applying bulk action:', error);
        }
      });
  }

  testConnection(): void {
    this.debugInfo = 'Testing API connection...';
    this.categoryService.getAllCategories()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (categories) => {
          this.categories = categories;
          this.debugInfo = `API Test Success! Loaded ${categories.length} categories`;
          console.log('Categories from API test:', categories);
        },
        error: (error) => {
          this.debugInfo = `API Test Failed: ${error.message || 'Unknown error'}`;
          console.error('API Test Error:', error);
        }
      });
  }
}