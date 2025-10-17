export interface Order {
    id: number;
    userId?: string;
    guestEmail?: string;
    guestName?: string;
    orderNumber: string;
    orderDate: Date;
    status: OrderStatus;
    subTotal: number;
    tax: number;
    shippingCost: number;
    totalAmount: number;
    shippingFirstName: string;
    shippingLastName: string;
    shippingAddress: string;
    shippingAddress2: string;
    shippingCity: string;
    shippingState: string;
    shippingZipCode: string;
    shippingCountry: string;
    shippingPhone: string;
    paymentMethod: string;
    paymentTransactionId: string;
    paymentProofImageUrl?: string;
    paymentDate?: Date;
    orderItems: OrderItem[];
    notes: string;
    fullShippingAddress: string;
    customerName: string;
}

export interface OrderItem {
    id: number;
    productId: number;
    productShadeId?: number;
    productShadeName?: string;
    quantity: number;
    unitPrice: number;
    totalPrice: number;
    productName: string;
    productImageUrl: string;
}

export interface CreateOrder {
    guestEmail?: string;
    guestName?: string;
    shippingFirstName: string;
    shippingLastName: string;
    shippingAddress: string;
    shippingAddress2: string;
    shippingCity: string;
    shippingState: string;
    shippingZipCode: string;
    shippingCountry: string;
    shippingPhone: string;
    paymentMethod: string;
    paymentProofImageUrl?: string;
    notes: string;
}

export enum OrderStatus {
    Pending = 0,
    Confirmed = 1,
    Completed = 2,
    Cancelled = 3
}
