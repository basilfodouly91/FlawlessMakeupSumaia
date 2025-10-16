import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, takeUntil, forkJoin } from 'rxjs';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

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
  currentLang = 'en';

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
    private categoryService: CategoryService,
    private translate: TranslateService
  ) {
    this.currentLang = this.translate.currentLang || this.translate.defaultLang || 'en';
    
    // Subscribe to language changes
    this.translate.onLangChange.pipe(takeUntil(this.destroy$)).subscribe((event) => {
      this.currentLang = event.lang;
    });
  }

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
          console.log('=== LOADED DATA FROM BACKEND ===');
          console.log('Products received:', data.products.length);
          console.log('First product:', data.products[0]);
          console.log('First product shades:', data.products[0]?.productShades);
          console.log('All products with shades:', data.products.filter(p => p.productShades && p.productShades.length > 0));
          
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
    console.log('showProductModal before:', this.showProductModal);
    console.log('Product to edit:', product);
    console.log('Product shades:', product?.productShades);
    
    this.editingProduct = product || null;
    this.productForm = product ? { 
      ...product,
      productShades: product.productShades && Array.isArray(product.productShades) 
        ? product.productShades.map(shade => ({
            id: shade.id, // Preserve ID for existing shades
            name: shade.name,
            stockQuantity: shade.stockQuantity,
            isActive: shade.isActive,
            displayOrder: shade.displayOrder
          })) 
        : [] as any[]
    } : this.getEmptyProductForm();
    
    this.showProductModal = true;
    console.log('showProductModal after:', this.showProductModal);
    console.log('Product form:', this.productForm);
    console.log('Product form shades:', this.productForm.productShades);
    
    // Force change detection
    setTimeout(() => {
      console.log('Modal should be visible now');
    }, 100);
  }

  closeProductModal(): void {
    this.showProductModal = false;
    this.editingProduct = null;
    this.productForm = this.getEmptyProductForm();
  }

  private getEmptyProductForm(): any {
    console.log('Creating empty product form');
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
      productShades: [] as any[],
      size: '',
      ingredients: '',
      skinType: '',
      isActive: true,
      isFeatured: false,
      isOnSale: false
    };
  }

  // Shade management methods
  addShade(): void {
    console.log('=== ADD SHADE CALLED ===');
    console.log('productForm:', this.productForm);
    console.log('Current productForm.productShades:', this.productForm.productShades);
    console.log('Is array?', Array.isArray(this.productForm.productShades));
    
    // Ensure productShades is initialized
    if (!this.productForm.productShades || !Array.isArray(this.productForm.productShades)) {
      console.log('Initializing productShades array');
      this.productForm.productShades = [];
    }
    
    const newShade = {
      name: '',
      stockQuantity: 0,
      isActive: true,
      displayOrder: this.productForm.productShades.length
    };
    
    console.log('New shade to add:', newShade);
    
    // Create a new array reference to trigger Angular change detection
    this.productForm.productShades = [...this.productForm.productShades, newShade];
    
    console.log('After add - productForm.productShades:', this.productForm.productShades);
    console.log('After add - length:', this.productForm.productShades.length);
    console.log('=== END ADD SHADE ===');
  }

  removeShade(index: number): void {
    console.log('Removing shade at index:', index);
    // Create new array without the removed item to trigger change detection
    this.productForm.productShades = this.productForm.productShades.filter((_: any, i: number) => i !== index);
    // Update display orders
    this.productForm.productShades.forEach((shade: any, idx: number) => {
      shade.displayOrder = idx;
    });
    console.log('After remove - shades count:', this.productForm.productShades.length);
  }

  moveShadeUp(index: number): void {
    if (index > 0) {
      console.log('Moving shade up from index:', index);
      // Create a new array with swapped items
      const newShades = [...this.productForm.productShades];
      const temp = newShades[index];
      newShades[index] = newShades[index - 1];
      newShades[index - 1] = temp;
      
      // Update display orders
      newShades.forEach((shade: any, idx: number) => {
        shade.displayOrder = idx;
      });
      
      this.productForm.productShades = newShades;
    }
  }

  moveShadeDown(index: number): void {
    if (index < this.productForm.productShades.length - 1) {
      console.log('Moving shade down from index:', index);
      // Create a new array with swapped items
      const newShades = [...this.productForm.productShades];
      const temp = newShades[index];
      newShades[index] = newShades[index + 1];
      newShades[index + 1] = temp;
      
      // Update display orders
      newShades.forEach((shade: any, idx: number) => {
        shade.displayOrder = idx;
      });
      
      this.productForm.productShades = newShades;
    }
  }

  saveProduct(): void {
    console.log('=== SAVE PRODUCT CALLED ===');
    console.log('Full productForm object:', JSON.stringify(this.productForm, null, 2));
    console.log('productForm.productShades:', this.productForm.productShades);
    console.log('productShades is array?', Array.isArray(this.productForm.productShades));
    console.log('productShades length:', this.productForm.productShades?.length);
    console.log('productShades content:', JSON.stringify(this.productForm.productShades, null, 2));

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
      productShades: this.productForm.productShades || [],
      size: this.productForm.size || '',
      ingredients: this.productForm.ingredients || '',
      skinType: this.productForm.skinType || ''
    };

    console.log('Processed product data:', JSON.stringify(productData, null, 2));
    console.log('Sending productShades:', JSON.stringify(productData.productShades, null, 2));

    const operation = this.editingProduct
      ? this.adminService.updateProduct(this.editingProduct.id, {
        ...productData,
        id: this.editingProduct.id,
        isActive: Boolean(this.productForm.isActive),
        productShades: this.productForm.productShades || []
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

  // Image upload handlers
  onImageSelect(event: Event, type: 'main' | 'additional'): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      
      // Validate file size (max 5MB)
      if (file.size > 5 * 1024 * 1024) {
        alert('Image size should be less than 5MB');
        return;
      }

      // Validate file type
      if (!file.type.startsWith('image/')) {
        alert('Please select an image file');
        return;
      }

      const reader = new FileReader();
      reader.onload = () => {
        const base64String = reader.result as string;
        if (type === 'main') {
          this.productForm.imageUrl = base64String;
        }
        // Additional images can be handled here if needed
      };
      reader.readAsDataURL(file);
    }
  }

  removeImage(type: 'main' | 'additional'): void {
    if (type === 'main') {
      this.productForm.imageUrl = '';
    }
  }

  getCategoryName(category: Category): string {
    return this.currentLang === 'ar' ? category.nameAr : category.nameEn;
  }
}