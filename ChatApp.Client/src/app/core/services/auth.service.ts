import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Login, Register } from '../models/auth.module';
import { User } from '../models/user.module';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}/auth`;
  currentUser = signal<User | null>(null);
  constructor(private http: HttpClient) { }

  // Lấy thông tin user
  getCurrentUser() {
    const userJson = localStorage.getItem('user');
    if (userJson) {
      const user = JSON.parse(userJson);
      this.currentUser.set(user);
    }
  }

  getUsers() {
    return this.http.get<User[]>(`${this.apiUrl}/GetUsers`);
  }

  // Lưu thông tin user
  setCurrentUser(user: User) {
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUser.set(user);
  }

  // Đăng nhập
  login(login: Login): Observable<User> {
    return this.http.post<User>(`${this.apiUrl}/login`, login);
  }
  // Đăng ký
  register(register: FormData) {
    return this.http.post(`${this.apiUrl}/register`, register);
  }

  // Đăng xuất
  logout() {
    localStorage.removeItem('user');
    this.currentUser.set(null);
  }
}
