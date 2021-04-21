import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
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
    },
    {
        path: 'not-found',
        component: NotFoundComponent
    },
    {
        path: 'server-error',
        component: ServerErrorComponent
    },
    {
        path: '**',
        component: NotFoundComponent,
        pathMatch: 'full'
    }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { relativeLinkResolution: 'legacy' })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
