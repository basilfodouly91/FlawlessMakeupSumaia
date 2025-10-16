import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

import { AdminService } from '../../services/admin.service';
import { AdminCategory } from '../../models/admin.model';

@Component({
  selector: 'app-category-management',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule],
  templateUrl: './category-management.html',
  styleUrl: './category-management.scss'
})
export class CategoryManagementComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  categories: AdminCategory[] = [];
  isLoading = true;

  // Modal
  showCategoryModal = false;
  editingCategory: AdminCategory | null = null;
  categoryFormData: any = this.getEmptyCategoryForm();

  constructor(
    private adminService: AdminService,
    private translate: TranslateService
  ) { }

  ngOnInit(): void {
    this.loadCategories();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadCategories(): void {
    this.isLoading = true;
    const currentLang = this.translate.currentLang || 'ar';

    this.adminService.getAllCategoriesForAdmin()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (categories) => {
          // Set the name field based on current language
          this.categories = categories.map(cat => ({
            ...cat,
            name: currentLang === 'ar' ? cat.nameAr : cat.nameEn
          })).sort((a, b) => a.displayOrder - b.displayOrder);
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Error loading categories:', error);
          this.isLoading = false;
        }
      });
  }

  refreshCategories(): void {
    this.loadCategories();
  }

  // Category CRUD operations
  openCategoryModal(category?: AdminCategory): void {
    this.editingCategory = category || null;
    this.categoryFormData = category ? { ...category } : this.getEmptyCategoryForm();
    this.showCategoryModal = true;
  }

  closeCategoryModal(): void {
    this.showCategoryModal = false;
    this.editingCategory = null;
    this.categoryFormData = this.getEmptyCategoryForm();
  }

  private getEmptyCategoryForm(): any {
    return {
      nameEn: '',
      nameAr: '',
      description: '',
      imageUrl: '',
      displayOrder: this.categories.length + 1,
      isActive: true
    };
  }

  saveCategory(): void {
    // Generate placeholder image if none provided
    if (!this.categoryFormData.imageUrl) {
      this.categoryFormData.imageUrl = `https://picsum.photos/400/300?random=${Date.now()}`;
    }

    const operation = this.editingCategory
      ? this.adminService.updateCategory(this.editingCategory.id, this.categoryFormData)
      : this.adminService.createCategory(this.categoryFormData);

    operation.pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.closeCategoryModal();
          this.refreshCategories();
        },
        error: (error) => {
          console.error('Error saving category:', error);
        }
      });
  }

  editCategory(category: AdminCategory): void {
    this.openCategoryModal(category);
  }

  deleteCategory(categoryId: number): void {
    const category = this.categories.find(c => c.id === categoryId);
    if (category && category.productCount > 0) {
      alert(`Cannot delete category "${category.name}" because it contains ${category.productCount} products. Please move or delete the products first.`);
      return;
    }

    if (confirm('Are you sure you want to delete this category?')) {
      this.adminService.deleteCategory(categoryId)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.refreshCategories();
          },
          error: (error) => {
            console.error('Error deleting category:', error);
          }
        });
    }
  }

  toggleActive(categoryId: number): void {
    this.adminService.toggleCategoryActive(categoryId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.refreshCategories();
        },
        error: (error) => {
          console.error('Error toggling category status:', error);
        }
      });
  }

  // Image upload handlers
  onImageSelect(event: Event): void {
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
        this.categoryFormData.imageUrl = base64String;
      };
      reader.readAsDataURL(file);
    }
  }

  removeImage(): void {
    this.categoryFormData.imageUrl = '';
  }
}