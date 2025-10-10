export interface Cart {
    id: number;
    userId: string;
    cartItems: CartItem[];
    dateCreated: Date;
    dateUpdated: Date;
    totalAmount: number;
    totalItems: number;
}

export interface CartItem {
    id: number;
    productId: number;
    productName: string;
    productImageUrl: string;
    productBrand: string;
    quantity: number;
    price: number;
    totalPrice: number;
    dateAdded: Date;
    isInStock: boolean;
    stockQuantity: number;
}

export interface AddToCart {
    productId: number;
    quantity: number;
}

export interface UpdateCartItem {
    productId: number;
    quantity: number;
}
