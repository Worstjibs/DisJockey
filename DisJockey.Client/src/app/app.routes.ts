import { Routes } from '@angular/router';
import { Home } from './home/home';
import { MemberList } from './members/components/member-list/member-list';
import { TrackList } from './tracks/components/track-list/track-list';
import { Search } from './search/components/search/search';

export const routes: Routes = [
	{
		path: '',
		component: Home
	},
	{
		path: 'members',
		component: MemberList
	},
	{
		path: 'tracks',
		component: TrackList
	},
	{
		path: 'search',
		component: Search
	}
];
