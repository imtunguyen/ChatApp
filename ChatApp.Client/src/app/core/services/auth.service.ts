import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { User } from '../models/user.module';
import { Observable } from 'rxjs';
import { Login } from '../models/auth.module';
import { StorageService } from '../../shared/services/storage.service';
import { ApiService } from '../../shared/services/api.service';
import { LoginResponse } from '../models/login-response.dto';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  currentUser = signal<User | null>(null);
  private storage = inject(StorageService);
  private api = inject(ApiService);
  constructor() { }

  // Lấy thông tin user
  getCurrentUser() {
    const user = this.storage.getItem('user');
    if (user) {
      return user;
    }
  }

  getUsers() {
    return this.api.get<User[]>('auth/GetUsers');
  }

  getUserById(id: string) {
    return this.api.get<User>('auth/GetUserById?id=' + id);
  }
  // Lưu thông tin user
  setCurrentUser(user: User) {
    this.storage.setItem('user', user);
    this.currentUser.set(user);
  }

  login(login: Login): Observable<LoginResponse> {
    return this.api.post<LoginResponse>('auth/login', login);
  }
  // Đăng ký
  register(register: FormData){
    return this.api.post('auth/register', register);
  }

  // Đăng xuất
  logout() {
    this.storage.removeItem('user');
    this.currentUser.set(null);
  }
}
