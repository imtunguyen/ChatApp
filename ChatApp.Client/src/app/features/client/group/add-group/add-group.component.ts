import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NzInputModule } from 'ng-zorro-antd/input';
import { User } from '../../../../core/models/user.module';
import { AuthService } from '../../../../core/services/auth.service';
import { GroupService } from '../../../../core/services/group.service';
import { ToastrService } from '../../../../shared/services/toastr.service';
@Component({
  selector: 'app-add-group',
  imports: [NzInputModule, CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './add-group.component.html',
  styleUrl: './add-group.component.scss'
})
export class AddGroupComponent implements OnInit {

  protected value = '';
  groupForm!: FormGroup;
  users: User[] = [];
  selectedUsers: any[] = [];

  selectedFiles: { src: string; file: File}[] = [];
  search: string = '';

  private authService = inject(AuthService);
  private groupService = inject(GroupService);
  private toastService = inject(ToastrService);
  private fb = inject(FormBuilder);
  constructor() {

   }
  ngOnInit() {
    this.loadUsers();
    this.groupForm = this.initializeForm();
  }

  initializeForm() {
    return this.fb.group({
      name: [''],
      users: ['']
    });
  }

  loadUsers() {
    this.authService.getUsers().subscribe((users) => {
      this.users = users;
    });
  }

  toggleUser(user: User, isChecked: boolean) {
    if (isChecked) {
      if (!this.selectedUsers.includes(user)) {
        this.selectedUsers.push(user);
      }
    } else {
      this.selectedUsers = this.selectedUsers.filter(u => u.id !== user.id);
    }
  }
  addGroup(){

    const formData = new FormData();
    formData.append('name', this.groupForm.get('name')?.value);
    formData.append('creatorId', this.authService.getCurrentUser().id);
    this.selectedUsers.forEach(user => {
      formData.append('userIds', user.id);
    })

    console.log(formData);
    this.groupService.addGroup(formData).subscribe(
      (res) => {
        console.log(res);
        this.toastService.showSuccess('Tạo nhóm thành công');
      },
      (err) => {
        console.log(err);
        this.toastService.showError('Tạo nhóm thất bại');
      }
    );
  }
  onFileSelected(event: any) {
    const files: FileList = event.target.files;
    const newFiles: { src: string; file: File}[] = [];

    for (let i = 0; i < files.length; i++) {
      const file = files[i];
      const reader = new FileReader();
      reader.onload = (e: any) => {
          newFiles.push({
            src: e.target.result,
            file,
          });

        if (i === files.length - 1) {
          this.selectedFiles = [...this.selectedFiles, ...newFiles];
        }
      };
      reader.readAsDataURL(file);

    }
  }
}
