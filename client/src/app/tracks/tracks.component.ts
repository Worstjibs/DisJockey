import { AfterViewChecked, Component, ElementRef, HostListener, OnInit, ViewChild, ViewChildren } from '@angular/core';
import { Observable } from 'rxjs';
import { TracksService } from '../_services/tracks.service';
import { Track } from '../_models/track';
import { UserParams } from '../_models/userParams';
import { PaginatedResult, Pagination } from '../_models/pagination';
import { TrackItemComponent } from './track-item/track-item.component';
import { ToastrService } from 'ngx-toastr';
import { BaseListComponent } from '../shared/base-list-component';

@Component({
	selector: 'app-tracks',
	templateUrl: './tracks.component.html',
	styleUrls: ['./tracks.component.css']
})
export class TracksComponent extends BaseListComponent<Track> implements OnInit {
	@ViewChildren('trackItem') trackItemComponents: TrackItemComponent[];

	get noMoreResults() {
		return this.pagination.currentPage == this.pagination.totalPages;
	}

	constructor(private tracksService: TracksService, private toastr: ToastrService) {       
        super();
	}
    
    loadServiceData(): Observable<PaginatedResult<Track>> {
        return this.tracksService.getTracks(this.userParams);
    }

	ngOnInit(): void {
        this.resetUserParams();
	}

	likeTrack(event): void {
		var track: Track = event.track;

		if (event.liked != track.likedByUser) {
			this.tracksService.postTrackLike(event.track, event.liked);
		}
	}

	playTrack(event): void {
		if (event.track) {
			this.tracksService.playTrack(event.track, event.playNow);
		}
	}

	trackExpanded(event: Track): void {
		// Close the current track
		const currentTrack = this.trackItemComponents.find(x => x.showDetail);
		if (currentTrack) {
			currentTrack.showDetail = false;
		}

		const requestedTrack = this.trackItemComponents.find(x => x.track.youtubeId == event.youtubeId);
		requestedTrack.showDetail = true;
	}
}
