import { EventEmitter } from '@angular/core';
import { Component, Input, OnInit, Output } from '@angular/core';
import { Track } from 'src/app/_models/track';

@Component({
	selector: 'app-track-item',
	templateUrl: './track-item.component.html',
	styleUrls: ['./track-item.component.css']
})
export class TrackItemComponent implements OnInit {
	@Input() track: Track;
	@Output() trackLiked = new EventEmitter<{ track: Track, liked: boolean }>();
	@Output() trackPlayed = new EventEmitter<{ track: Track, playNow: boolean }>();
	@Output() toggledDetail = new EventEmitter<Track>();

	showDetail: boolean;

	constructor() { }

	ngOnInit(): void {
		this.showDetail = false;
	}

	likeTrack(track: Track) {
		event.stopPropagation();
		this.trackLiked.emit({ track, liked: true });
	}

	dislikeTrack(track: Track) {
		event.stopPropagation();
		this.trackLiked.emit({ track, liked: false });
	}

	playTrack(track: Track) {
		event.stopPropagation();
		this.trackPlayed.emit({ track, playNow: true });
	}

	queueTrack(track: Track) {
		event.stopPropagation();
		this.trackPlayed.emit({ track, playNow: false });
	}

	toggleDetail() {
		if (!this.showDetail) {
			this.toggledDetail.emit(this.track);
		} else {
			this.showDetail = false;
		}
	}

}
