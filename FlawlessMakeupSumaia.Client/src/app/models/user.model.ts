export interface User {
    id: string;
    email: string;
    firstName: string;
    lastName: string;
    dateCreated: Date;
    fullName: string;
    roles: string[];
}

export interface AuthResponse {
    success: boolean;
    token?: string;
    message?: string;
    user?: User;
}

export interface RegisterRequest {
    email: string;
    password: string;
    confirmPassword: string;
    firstName: string;
    lastName: string;
}

export interface LoginRequest {
    email: string;
    password: string;
}
