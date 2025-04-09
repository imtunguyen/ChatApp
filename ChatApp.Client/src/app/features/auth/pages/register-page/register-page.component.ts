import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzFormModule } from 'ng-zorro-antd/form';
import { AuthService } from '../../../../core/services/auth.service';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { GENDER_LIST } from '../../../../core/models/enum/gender';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { ToastrService } from '../../../../shared/services/toastr.service';
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

  previewImage: string | ArrayBuffer | null = null;
  selectedFile: File | null = null;

  private toastrService = inject(ToastrService);
  constructor(private authService: AuthService, private router: Router, private fb: FormBuilder) {}


  ngOnInit() {
    this.initForm();
    this.registerForm.get('password')?.valueChanges.subscribe(() => {
      this.registerForm.get('confirmPassword')?.updateValueAndValidity();
    });
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
        profilePicture: [''],
      },
      { validator: this.checkPasswords }
    );
  }

  checkPasswords = (group: FormGroup): { [key: string]: any } | null => {
    const password = group.get('password')?.value;
    const confirmPassword = group.get('confirmPassword')?.value;
    if (password && confirmPassword && password !== confirmPassword) {
      return { notSame: true };
    }
    return null;
  }
  
  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      this.selectedFile = input.files[0];
  
      const reader = new FileReader();
      reader.onload = (e) => {
        this.previewImage = e.target?.result ?? null;
      };
      reader.readAsDataURL(this.selectedFile);
    }
  }

  onRegister() {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched(); // hiển thị lỗi rõ ràng
      this.toastrService.showError('Vui lòng kiểm tra lại thông tin đăng ký!');
      return;
    }
    const formData = new FormData();
    formData.append('fullName', this.registerForm.get('fullName')?.value);
    formData.append('userName', this.registerForm.get('userName')?.value);
    formData.append('email', this.registerForm.get('email')?.value);
    formData.append('password', this.registerForm.get('password')?.value);
    formData.append('gender', this.registerForm.get('gender')?.value);
    if (this.selectedFile) {
      formData.append('profilePicture', this.selectedFile);
    }
    
    this.authService.register(formData).subscribe({
      next: (response) => {
        this.toastrService.showSuccess('Registration successful');
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 2000);
        
      },
      error: (error) => {
        console.error('Registration failed:', error);
    
        // Hiển thị message cụ thể từ backend
        this.toastrService.showError(error.message || 'Đăng ký thất bại. Vui lòng thử lại.');
      }
      
    });
  }
  navigateToLogin() {
    this.router.navigate(['/login']);
  }
}