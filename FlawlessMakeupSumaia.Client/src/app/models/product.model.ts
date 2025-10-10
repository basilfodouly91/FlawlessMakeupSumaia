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
    size?: string;
    ingredients?: string;
    skinType?: string;
    currentPrice: number;
    hasDiscount: boolean;
    discountPercentage: number;
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
    size?: string;
    ingredients?: string;
    skinType?: string;
}
