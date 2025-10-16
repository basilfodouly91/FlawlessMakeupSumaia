export interface ProductShade {
    id: number;
    productId: number;
    name: string;
    stockQuantity: number;
    isActive: boolean;
    displayOrder: number;
    dateCreated: Date;
    dateUpdated: Date;
}

export interface Product {
    id: number;
    name: string;
    description: string;
    price: number;
    salePrice?: number;
    stockQuantity: number;
    imageUrl: string;
    imageUrls: string[];
    categoryId: number;
    categoryName: string;
    isActive: boolean;
    isFeatured: boolean;
    isOnSale: boolean;
    dateCreated: Date;
    dateUpdated: Date;
    brand?: string;
    shade?: string;
    productShades: ProductShade[];
    size?: string;
    ingredients?: string;
    skinType?: string;
    currentPrice: number;
    hasDiscount: boolean;
    discountPercentage: number;
}

export interface CreateProductShade {
    id?: number; // Optional: for existing shades during update
    name: string;
    stockQuantity: number;
    isActive: boolean;
    displayOrder: number;
}

export interface CreateProduct {
    name: string;
    description: string;
    price: number;
    salePrice?: number;
    stockQuantity: number;
    imageUrl: string;
    imageUrls: string[];
    categoryId: number;
    isFeatured: boolean;
    isOnSale: boolean;
    brand?: string;
    shade?: string;
    productShades?: CreateProductShade[];
    size?: string;
    ingredients?: string;
    skinType?: string;
}
