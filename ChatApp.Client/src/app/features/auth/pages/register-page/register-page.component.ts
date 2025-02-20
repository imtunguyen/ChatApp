import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzFormModule } from 'ng-zorro-antd/form';
import { AuthService } from '../../../../core/services/auth.service';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { GENDER_LIST } from '../../../../core/models/enum/gender';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzIconModule } from 'ng-zorro-antd/icon';
@Component({
  imports: [CommonModule, NzCardModule, NzFormModule, ReactiveFormsModule, FormsModule, NzSelectModule,  NzInputModule, NzIconModule],
  selector: 'app-register-page',
  templateUrl: './register-page.component.html',
  styleUrl: './register-page.component.scss',
})
export class RegisterPageComponent implements OnInit {
  
  registerForm!: FormGroup;
  list = GENDER_LIST;
  passwordVisible = false;
  confirmPasswordVisible = false;
  constructor(private authService: AuthService, private router: Router, private fb: FormBuilder) {}


  ngOnInit() {
    this.initForm();
  }

  initForm(): void {
    this.registerForm = this.fb.group(
      {
        fullName: ['', Validators.required],
        userName: ['', Validators.required],
        email: ['', [Validators.required, Validators.email]],
        gender: ['', Validators.required],
        password: ['', Validators.required],
        confirmPassword: ['', Validators.required],
        profilePicture: ['', Validators.required],
      },
      { validator: this.checkPasswords }
    );
  }

  checkPasswords(group: FormGroup): { [key: string]: any } | null {
    const password = group.get('password')?.value;
    const confirmPassword = group.get('confirmPassword')?.value;
    return password === confirmPassword ? null : { notSame: true };
  }

  onRegister() {
    const formData = new FormData();
    formData.append('fullName', this.registerForm.get('fullName')?.value);
    formData.append('userName', this.registerForm.get('userName')?.value);
    formData.append('email', this.registerForm.get('email')?.value);
    formData.append('password', this.registerForm.get('password')?.value);
    formData.append('gender', this.registerForm.get('gender')?.value);
    formData.append('profilePicture', this.registerForm.get('profilePicture')?.value);
    this.authService.register(formData).subscribe({
      next: (response) => {
        this.router.navigate(['/login']); // Chuyển hướng đến trang đăng nhập sau khi đăng ký thành công
      },
      error: (error) => {
        console.error('Registration failed', error);
      },
    });
  }
  navigateToLogin() {
    this.router.navigate(['/login']);
  }
}