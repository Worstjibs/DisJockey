import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { TracksComponent } from './tracks/tracks.component';
import { UsersComponent } from './users/users.component';

const routes: Routes = [
    {
        path: '',
        component: HomeComponent
    },
    {
        path: 'tracks',
        component: TracksComponent
    },
    {
        path: 'users',
        component: UsersComponent
    }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
