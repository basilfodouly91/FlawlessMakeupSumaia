export interface Order {
    id: number;
    userId: string;
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
    paymentDate?: Date;
    orderItems: OrderItem[];
    notes: string;
    fullShippingAddress: string;
    customerName: string;
}

export interface OrderItem {
    id: number;
    productId: number;
    quantity: number;
    unitPrice: number;
    totalPrice: number;
    productName: string;
    productImageUrl: string;
}

export interface CreateOrder {
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
    notes: string;
}

export enum OrderStatus {
    Pending = 0,
    Processing = 1,
    Shipped = 2,
    Delivered = 3,
    Cancelled = 4,
    Refunded = 5
}
