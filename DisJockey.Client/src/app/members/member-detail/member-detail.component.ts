import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { map, take } from 'rxjs/operators';
import { TrackListType } from '../../_enums/trackListType';
import { Member } from '../../_models/member';
import { Playlist } from '../../_models/playlist';
import { AccountService } from '../../_services/account.service';
import { PlaylistsService } from '../../_services/playlists.service';

@Component({
	selector: 'app-member-detail',
	templateUrl: './member-detail.component.html',
	styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
	member: Member;

	TrackListType = TrackListType;

	constructor(
		private readonly route: ActivatedRoute,
		private readonly accountService: AccountService
	) { }

	get isPlaylistsEnabled() {
		return this.accountService.currentUser$.pipe(map(user => user.discordId === this.member.discordId || this.member.playlists.length > 0));
    }

	ngOnInit(): void {
		this.route.data.subscribe((data: any) => {
			this.member = data.member as Member;
		});
	}

}
