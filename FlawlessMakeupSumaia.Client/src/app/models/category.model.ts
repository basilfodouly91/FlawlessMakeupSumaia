export interface Category {
    id: number;
    name: string;
    description: string;
    imageUrl: string;
    isActive: boolean;
    displayOrder: number;
    dateCreated: Date;
    productCount: number;
}
