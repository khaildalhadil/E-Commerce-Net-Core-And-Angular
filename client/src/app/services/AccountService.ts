import { HttpClient } from '@angular/common/http';
import { inject, Service, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { User } from '../shared/models/user';

const baseURL = "http://localhost:5130/api/Account";
const STORAGE_KEY = "user";

@Service()
export class AccountService {
    private httpClient = inject(HttpClient);
    currentUser = signal<User | null>(null);

    constructor() {
        const storedUser = localStorage.getItem(STORAGE_KEY);
        if (storedUser) {
            this.currentUser.set(JSON.parse(storedUser));
        }
    }

    login(values: { email: string, password: string }): Observable<User> {
        return this.httpClient.post<User>(`${baseURL}/Login`, values).pipe(
            tap(user => this.setCurrentUser(user))
        );
    }

    register(values: { displayName: string, email: string, password: string }): Observable<User> {
        return this.httpClient.post<User>(`${baseURL}/Register`, values).pipe(
            tap(user => this.setCurrentUser(user))
        );
    }

    logout() {
        localStorage.removeItem(STORAGE_KEY);
        this.currentUser.set(null);
    }

    private setCurrentUser(user: User) {
        localStorage.setItem(STORAGE_KEY, JSON.stringify(user));
        this.currentUser.set(user);
    }
}
