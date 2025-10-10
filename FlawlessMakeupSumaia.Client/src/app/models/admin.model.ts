export interface AdminDashboard {
    totalProducts: number;
    activeProducts: number;
    featuredProducts: number;
    productsOnSale: number;
    totalCategories: number;
    activeCategories: number;
    lowStockProducts: number;
    outOfStockProducts: number;
}

export interface AdminProduct {
    id: number;
    name: string;
    description: string;
    price: number;
    salePrice?: number;
    stockQuantity: number;
    imageUrl: string;
    categoryId: number;
    categoryName: string;
    brand?: string;
    isActive: boolean;
    isFeatured: boolean;
    isOnSale: boolean;
    dateCreated: Date;
    dateUpdated: Date;
    status: string;
}

export interface AdminCategory {
    id: number;
    name: string;
    description: string;
    imageUrl: string;
    displayOrder: number;
    isActive: boolean;
    dateCreated: Date;
    productCount: number;
}

export interface BulkUpdate {
    productIds: number[];
    action: string;
    salePrice?: number;
}

export interface SaleToggle {
    isOnSale: boolean;
    salePrice?: number;
}

export interface StockUpdate {
    quantity: number;
}

export interface ProductAnalytics {
    totalProducts: number;
    productsByCategory: CategoryProductCount[];
    productsByBrand: BrandProductCount[];
    stockStatus: StockStatus;
}

export interface CategoryProductCount {
    categoryName: string;
    count: number;
}

export interface BrandProductCount {
    brandName: string;
    count: number;
}

export interface StockStatus {
    inStock: number;
    lowStock: number;
    outOfStock: number;
}
