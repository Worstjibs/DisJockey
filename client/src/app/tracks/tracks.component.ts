import { AfterViewChecked, Component, ElementRef, HostListener, OnInit, ViewChild, ViewChildren } from '@angular/core';
import { Observable } from 'rxjs';
import { TracksService } from '../_services/tracks.service';
import { Track } from '../_models/track';
import { UserParams } from '../_models/userParams';
import { Pagination } from '../_models/pagination';
import { TrackItemComponent } from './track-item/track-item.component';

@Component({
	selector: 'app-tracks',
	templateUrl: './tracks.component.html',
	styleUrls: ['./tracks.component.css']
})
export class TracksComponent implements OnInit {
	tracks: Track[];
	userParams: UserParams;
	pagination: Pagination;

	@ViewChildren('trackItem') trackItemComponents: TrackItemComponent[];

	constructor(private tracksService: TracksService) {
		this.userParams = tracksService.resetUserParams();
	}

	ngOnInit(): void {
		this.userParams = new UserParams();
		this.tracks = [];

		this.getTracks();
	}

	getTracks(): void {
		this.tracksService.getTracks(this.userParams).subscribe(response => {
			this.pagination = response.pagination;
			this.tracks = this.tracks.concat(response.result);
		});
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

	toggledDetail(event: Track): void {
		// Close the current track
		const currentTrack = this.trackItemComponents.find(x => x.showDetail);
		if (currentTrack) {
			currentTrack.showDetail = false;
		}

		const requestedTrack = this.trackItemComponents.find(x => x.track.youtubeId == event.youtubeId);
		requestedTrack.showDetail = true;
	}

	sortBy(predicate: string) {
		this.userParams = new UserParams();
		this.userParams.sortBy = predicate;
		this.tracks = [];

		this.getTracks();
	}

	loadMore(): void {
		this.userParams.pageNumber++;

		this.getTracks();
	}
}
