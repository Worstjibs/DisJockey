import { Component, Input, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseListComponent } from '../../shared/base-list-component';
import { Member } from '../../_models/member';
import { PaginatedResult } from '../../_models/pagination';
import { Track } from '../../_models/track';
import { TracksService } from '../../_services/tracks.service';

@Component({
	selector: 'app-member-track-list',
	templateUrl: './member-track-list.component.html',
	styleUrls: ['./member-track-list.component.css']
})
export class MemberTrackListComponent extends BaseListComponent<Track> implements OnInit {
	@Input() member: Member;

	constructor(private readonly tracksService: TracksService) {
		super();
	}

    ngOnInit(): void {
		this.resetUserParams();
    }

	loadServiceData(): Observable<PaginatedResult<Track>> {
		return this.tracksService.getTrackPlaysForMember(this.userParams, this.member.discordId);
	}

}
