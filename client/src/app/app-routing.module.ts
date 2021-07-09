import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { HomeComponent } from './home/home.component';
import { TracksComponent } from './tracks/tracks.component';
import { MembersComponent } from './members/members.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberDetailResolver } from './_resolvers/member-detail.resolver';

const routes: Routes = [
	{
		path: '',
		component: HomeComponent
	},
	{
		path: '',
		runGuardsAndResolvers: 'always',
		children: [
			{ path: 'tracks', component: TracksComponent },
			{ path: 'users', component: MembersComponent },
			{ path: 'users/:discordId', component: MemberDetailComponent, resolve: { member: MemberDetailResolver } }
		]
	},
	{ path: 'not-found', component: NotFoundComponent },
	{ path: 'server-error', component: ServerErrorComponent },
	{ path: '**', component: NotFoundComponent, pathMatch: 'full' }
];

@NgModule({
	imports: [RouterModule.forRoot(routes)],
	exports: [RouterModule]
})
export class AppRoutingModule { }
