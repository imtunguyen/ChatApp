import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { LoginDto } from '../../../../core/models/login';
import { AuthService } from '../../../../core/services/auth.service';
import { LoginResponse } from '../../../../core/models/login-response.dto';
import { CommonModule } from '@angular/common';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzFormModule } from 'ng-zorro-antd/form';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ToastrService } from '../../../../shared/services/toastr.service';
@Component({
  imports: [CommonModule, NzCardModule, NzFormModule, ReactiveFormsModule, FormsModule],
  selector: 'app-login-page',
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.scss'
})
export class LoginPageComponent {
  loginDto: LoginDto = {
    userNameOrEmail: '',
    password: '',
  };
  private toastrService = inject(ToastrService);
  private authService = inject(AuthService);
  constructor(private router: Router) {}

  onLogin() {
    this.authService.login(this.loginDto).subscribe({
      next: (response: LoginResponse) => {
        localStorage.setItem('accessToken', response.accessToken);
        localStorage.setItem('refreshToken', response.refreshToken);

        localStorage.setItem('user', JSON.stringify(response.user));

        this.router.navigate(['/']);
      },
      error: (error) => {
        this.toastrService.showError('Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin đăng nhập.');
      },
    });
  }

  navigateToRegister() {
    this.router.navigate(['/register']);
  }
}