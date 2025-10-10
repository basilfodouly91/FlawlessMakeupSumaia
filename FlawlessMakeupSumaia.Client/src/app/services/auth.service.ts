import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { ApiService } from './api.service';
import { AuthResponse, LoginRequest, RegisterRequest, User } from '../models/user.model';

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private currentUserSubject = new BehaviorSubject<User | null>(null);
    public currentUser$ = this.currentUserSubject.asObservable();

    private isLoggedInSubject = new BehaviorSubject<boolean>(false);
    public isLoggedIn$ = this.isLoggedInSubject.asObservable();

    constructor(
        private apiService: ApiService,
        private router: Router
    ) {
        this.checkAuthStatus();
    }

    register(request: RegisterRequest): Observable<AuthResponse> {
        return this.apiService.post<AuthResponse>('api/auth/register', request).pipe(
            tap(response => {
                if (response.success && response.token && response.user) {
                    this.setAuthData(response.token, response.user);
                }
            })
        );
    }

    login(request: LoginRequest): Observable<AuthResponse> {
        return this.apiService.post<AuthResponse>('api/auth/login', request).pipe(
            tap(response => {
                if (response.success && response.token && response.user) {
                    this.setAuthData(response.token, response.user);
                }
            })
        );
    }

    logout(): void {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        this.currentUserSubject.next(null);
        this.isLoggedInSubject.next(false);
        this.router.navigate(['/']);
    }

    getCurrentUser(): User | null {
        return this.currentUserSubject.value;
    }

    isLoggedIn(): boolean {
        return this.isLoggedInSubject.value;
    }

    getToken(): string | null {
        return localStorage.getItem('token');
    }

    private setAuthData(token: string, user: User): void {
        localStorage.setItem('token', token);
        localStorage.setItem('user', JSON.stringify(user));
        this.currentUserSubject.next(user);
        this.isLoggedInSubject.next(true);
    }

    private checkAuthStatus(): void {
        const token = localStorage.getItem('token');
        const userStr = localStorage.getItem('user');

        if (token && userStr) {
            try {
                const user = JSON.parse(userStr) as User;
                this.currentUserSubject.next(user);
                this.isLoggedInSubject.next(true);
            } catch (error) {
                this.logout();
            }
        }
    }
}
