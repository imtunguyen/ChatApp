import {ChangeDetectionStrategy, Component, inject, OnInit} from '@angular/core';
import {FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import {TuiButton, TuiLink, TuiNotification} from '@taiga-ui/core';
import {TuiComboBoxModule, TuiInputModule} from '@taiga-ui/legacy';
import { trigger, state, style, transition, animate } from '@angular/animations';
import { AuthService } from '../../../../core/services/auth.service';
import { CommonModule } from '@angular/common';
import { Gender } from '../../../../core/models/gender';
import { TuiDataListWrapper, TuiFilterByInputPipe } from '@taiga-ui/kit';
@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, TuiInputModule, TuiButton, FormsModule, CommonModule, TuiComboBoxModule, TuiDataListWrapper],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  animations: [
    trigger('fadeInOut', [
      state('void', style({ opacity: 0, transform: 'translateY(20px)' })),
      state('*', style({ opacity: 1, transform: 'translateY(0)' })),
      transition('void => *', [animate('0.5s ease-in')]),
      transition('* => void', [animate('0.3s ease-out')]),
    ]),
  ],
})
export class RegisterComponent implements OnInit {
  registerForm!: FormGroup;
  selectedFile: { src: string; file: File} | undefined;
  genderOptions: string[] = [];
  private authService = inject(AuthService);
  constructor(private fb: FormBuilder) {
    this.registerForm = this.initializeRegisterForm();
    this.genderOptions = Object.values(Gender);
    console.log("gender", this.genderOptions);
  }
  ngOnInit(): void {
    console.log("hi");
  }

  initializeRegisterForm() {
    return this.fb.group({
      fullName: ['', Validators.required],
      userName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
      confirmPassword: ['', Validators.required],
      gender: ['', Validators.required],
      profilePicture: [''],
    });
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.selectedFile =
        { src: e.target.result,
          file: file,
        };
      };
      reader.readAsDataURL(file);
      this.registerForm.get('profilePicture')?.setValue(file);
    }
  }

  onSubmit(): void {
    console.log("register");

    const formData = new FormData();
    formData.append('fullName', this.registerForm.get('fullName')?.value);
    formData.append('userName', this.registerForm.get('userName')?.value);
    formData.append('email', this.registerForm.get('email')?.value);
    formData.append('password', this.registerForm.get('password')?.value);
    formData.append('gender', this.registerForm.get('gender')?.value);
    formData.append('profilePicture', this.registerForm.get('profilePicture')?.value);

    this.authService.register(formData).subscribe({
      next: (res) => {
        console.log(res);
        //this.authService.saveToken(res.accessToken);
        //this.authService.showSuccess('Register successfully').subscribe();
      },
      error: (err) => {
        //this.authService.showError('Register failed').subscribe();
      },

    });

  }
}
