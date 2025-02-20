import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { StorageService } from '../../services/storage.service';

@Component({
  selector: 'app-sidebar-nav',
  imports: [RouterLink, RouterOutlet, NzIconModule, NzLayoutModule, NzMenuModule],
  templateUrl: './sidebar-nav.component.html',
  styleUrl: './sidebar-nav.component.scss'
})
export class SidebarNavComponent {
  isCollapsed = false;

  private storageService = inject(StorageService);
  private router = inject(Router);
  logout() {
    this.storageService.removeItem('accessToken');
    this.storageService.removeItem('refreshToken');
    this.storageService.removeItem('user');

    this.router.navigate(['/login']);
  }
}
