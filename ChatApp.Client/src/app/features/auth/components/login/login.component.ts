import {ChangeDetectionStrategy, Component, inject, OnInit} from '@angular/core';
import {FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {TuiLink, TuiNotification} from '@taiga-ui/core';
import {TuiInputModule} from '@taiga-ui/legacy';
import { trigger, state, style, transition, animate } from '@angular/animations';
import { AuthService } from '../../../../core/services/auth.service';
import { NotificationService } from '../../../../shared/services/notification.service';
import { User } from '../../../../core/models/user.module';
import { Router } from '@angular/router';


@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, TuiInputModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
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
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;

  private authService = inject(AuthService);
  private notificationService = inject(NotificationService);
  constructor(private fb: FormBuilder, private router: Router) {
    this.loginForm = this.initializeLoginForm();
  }
  ngOnInit(): void {

  }

  initializeLoginForm() {
    return this.fb.group({
      userNameOrEmail: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      return;
    }
    const login = this.loginForm.value;
    console.log(login);
    this.authService.login(login).subscribe(
      (user: User) => {
        this.authService.setCurrentUser(user);
        this.notificationService.showSuccess('Đăng nhập thành công');
        this.router.navigate(['/chat']);
      },
      (error) => {
        this.notificationService.showError('Đăng nhập thất bại');
        console.log(error);
      }
    );

  }
}
