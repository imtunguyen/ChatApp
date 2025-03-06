import { Component, inject, OnInit } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { AuthService } from '../../../core/services/auth.service';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { GENDER_LIST } from '../../../core/models/enum/gender';
import { profile } from 'console';
import { User } from '../../../core/models/user.module';
import { ToastrService } from '../../services/toastr.service';
@Component({
  selector: 'app-sidebar-nav',
  imports: [RouterLink, RouterOutlet, NzIconModule, NzLayoutModule, NzMenuModule, NzModalModule, CommonModule, FormsModule, ReactiveFormsModule, NzSelectModule],
  templateUrl: './sidebar-nav.component.html',
  styleUrl: './sidebar-nav.component.scss',

  
})
export class SidebarNavComponent {
  isCollapsed = false;
  isVisible = false;
  isEditing = false;
  currentUser: any;
  list = GENDER_LIST;
  userForm!: FormGroup;

  previewImage: string | ArrayBuffer | null = null;
  selectedFile: File | null = null;

  private authService = inject(AuthService);
  private toastrService = inject(ToastrService);
  private router = inject(Router);
  constructor(private fb: FormBuilder) {
    this.currentUser = this.authService.getCurrentUser();
  }

  ngOnInit() {
    this.initForm();
    this.userForm = this.fb.group({
      fullName: [{ value: this.currentUser?.fullName, disabled: true }, Validators.required],
      email: [{ value: this.currentUser?.email, disabled: true }, [Validators.required, Validators.email]],
      gender: [{ value: this.currentUser?.gender, disabled: true }],
      profilePicture: [{ value: this.currentUser?.profilePicture, disabled: true }],
    });
  }

  initForm(): void {
    this.userForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      fullName: ['', Validators.required],
      profilePicture: [''],
      gender: [''],
    });
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  showModal(): void {
    this.isVisible = true;
  }
  handleCancel(): void {
    console.log('Button cancel clicked!');
    this.isVisible = false;
  }

  toggleUpdate() {
    this.isEditing = !this.isEditing;
  if (this.isEditing) {
    this.userForm.enable(); 
  } else {
    this.userForm.disable(); 
  }
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

  saveChanges() {
    const formData = new FormData();
    formData.append('id', this.currentUser.id);
    formData.append('fullName', this.userForm.get('fullName')?.value);
    formData.append('email', this.userForm.get('email')?.value);
    formData.append('gender', this.userForm.get('gender')?.value);
    if (this.selectedFile) {
      formData.append('profilePicture', this.selectedFile);
    }
    

    this.authService.updateUser(formData).subscribe({
      next: (response: any): void => {
        this.isEditing = false;
        setTimeout(() => {
          this.toastrService.showSuccess('Cập nhật thông tin thành công');
        }, 500);
        this.isVisible = false;
        this.authService.setCurrentUser(response);
      },
      error: (error) => {
        console.error('Update failed', error);
      },
    });
  }
}
