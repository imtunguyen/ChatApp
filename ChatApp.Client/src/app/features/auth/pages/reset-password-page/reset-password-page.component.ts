import { Component, inject, OnInit } from '@angular/core';
import { ToastrService } from '../../../../shared/services/toastr.service';
import { AuthService } from '../../../../core/services/auth.service';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzFormModule } from 'ng-zorro-antd/form';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';


@Component({
  selector: 'app-reset-password-page',
  imports: [CommonModule, NzCardModule, NzFormModule, ReactiveFormsModule, FormsModule],
  templateUrl: './reset-password-page.component.html',
  styleUrl: './reset-password-page.component.scss'
})
export class ResetPasswordPageComponent implements OnInit {

  email: string = '';
  token: string = '';
  newPassword: string = '';
  confirmPassword: string = '';

  private toastrService = inject(ToastrService);
  private authService = inject(AuthService);
  constructor(private route: ActivatedRoute, private router: Router) {}

 

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.email = params['email'];
      this.token = params['token'];
    });
  }

  onResetPassword() {
    if (!this.newPassword || this.newPassword.length < 6) {
      this.toastrService.showWarning('Mật khẩu mới phải có ít nhất 6 ký tự!');
      return;
    }

    if (this.newPassword !== this.confirmPassword) {
      this.toastrService.showWarning('Mật khẩu xác nhận không khớp!');
      return;
    }

    const resetData = {
      email: this.email,
      token: this.token,
      newPassword: this.newPassword,
    };

    this.authService.resetPassword(resetData).subscribe({
      next: (res) => {
        this.toastrService.showSuccess('Đổi mật khẩu thành công! Vui lòng đăng nhập lại.');
        this.router.navigate(['/login']);
      },
      error: () => {
        this.toastrService.showError('Token không hợp lệ hoặc đã hết hạn!');
      },
    });
  }

  navigateToLogin() {
    this.router.navigate(['/login']);
  }
}
