import { Routes } from '@angular/router';
import { SidebarNavComponent } from './shared/components/sidebar-nav/sidebar-nav.component';

export const routes: Routes = [

  { path: '', component: SidebarNavComponent, children: [
    { path: 'chat', loadChildren: () => import('./features/chat/chat.module').then(m => m.ChatModule) },

  ]},
  { path: '', loadChildren: () => import('./features/auth/auth.module').then(m => m.AuthModule) },
  // { path: 'welcome', loadChildren: () => import('./pages/welcome/welcome.routes').then(m => m.WELCOME_ROUTES) }
];
