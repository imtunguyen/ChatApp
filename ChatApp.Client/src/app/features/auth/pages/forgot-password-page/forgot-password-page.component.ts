import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzFormModule } from 'ng-zorro-antd/form';
import { ToastrService } from '../../../../shared/services/toastr.service';
import { AuthService } from '../../../../core/services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-forgot-password-page',
  imports: [CommonModule, NzCardModule, NzFormModule, ReactiveFormsModule, FormsModule],
  templateUrl: './forgot-password-page.component.html',
  styleUrl: './forgot-password-page.component.scss'
})
export class ForgotPasswordPageComponent {
  email: string = '';

  private toastrService = inject(ToastrService);
  private authService = inject(AuthService);
  constructor(private router: Router) {}

  onForgotPassword() {
    if(this.email.trim() == ''){
      this.toastrService.showError('Vui lòng nhập email');
      return;
    }

    this.authService.forgotPassword(this.email).subscribe({
      next: (res) => {
        this.toastrService.showSuccess(res.message || 'Vui lòng kiểm tra email để đặt lại mật khẩu');
      },
      error: (error) => {
        this.toastrService.showError('Đặt lại mật khẩu thất bại. Vui lòng kiểm tra lại email');
      },
    });
  }
  navigateToLogin() {
    this.router.navigate(['/login']);
  }
}
