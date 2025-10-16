export interface Category {
    id: number;
    nameEn: string;
    nameAr: string;
    description: string;
    imageUrl: string;
    isActive: boolean;
    displayOrder: number;
    dateCreated: Date;
    productCount: number;
}
