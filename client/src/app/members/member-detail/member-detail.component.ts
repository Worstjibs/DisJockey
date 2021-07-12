import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Member } from '../../_models/member';
import { Playlist } from '../../_models/playlist';
import { PlaylistsService } from '../../_services/playlists.service';

@Component({
	selector: 'app-member-detail',
	templateUrl: './member-detail.component.html',
	styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
	member: Member;

	constructor(
		private readonly route: ActivatedRoute
	) { }

	ngOnInit(): void {
		this.route.data.subscribe((data: any) => {
			this.member = data.member as Member;
		});
	}

}
