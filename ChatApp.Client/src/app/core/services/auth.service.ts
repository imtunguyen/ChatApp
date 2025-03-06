import { inject, Injectable, signal } from '@angular/core';
import { User } from '../models/user.module';
import { Observable, tap } from 'rxjs';
import { Login } from '../models/auth.module';
import { StorageService } from '../../shared/services/storage.service';
import { ApiService } from '../../shared/services/api.service';
import { LoginResponse, UserDto } from '../models/login-response.dto';
import { SignalRService } from './signalr.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  currentUser = signal<User | null>(null);
  private storage = inject(StorageService);
  private api = inject(ApiService);
  private signalR = inject(SignalRService);

  constructor() { }

  getCurrentUser() {
    const user = localStorage.getItem('user');
    if (user) {
        try {
            return JSON.parse(user); 
        } catch (error) {
            console.error('Lỗi khi parse JSON user:', error);
            return null; 
        }
    }
    return null;
}


  getUsers() {
    return this.api.get<User[]>('auth/GetUsers');
  }

  getUserById(id: string) {
    return this.api.get<User>('auth/GetUserById?id=' + id);
  }

  setCurrentUser(user: User) {
    this.storage.setItem('user', JSON.stringify(user));
    this.currentUser.set(user);
  }

  login(login: Login): Observable<LoginResponse> {
    return this.api.post<LoginResponse>('auth/login', login).pipe(
      tap((response : any) => {
        console.log('User logged in:', response.user.id);
        this.currentUser.set(response.user);
        this.storage.setItem('user', JSON.stringify(response.user));
        this.signalR.startConnection();
      })
    );
  }

  register(register: FormData) {
    return this.api.post('auth/register', register);
  }

  updateUser(user: FormData) {
    return this.api.put('auth/updateUser', user);
  }

  logout() {
    const currentUser = this.getCurrentUser();
    if (!currentUser || !currentUser.id) {
      return;
    }

    this.currentUser.set(null);
    this.storage.removeItem('user');
    this.signalR.stopConnection();
  }

  getOnlineUsers() {
    this.signalR.getOnlineUsers();
  }
}
