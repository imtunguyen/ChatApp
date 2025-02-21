import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { FriendshipPageComponent } from './pages/friendship-page/friendship-page.component';

const routes: Routes = [
  { path: '', component: FriendshipPageComponent },
];


@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class FriendshipRoutingModule {}