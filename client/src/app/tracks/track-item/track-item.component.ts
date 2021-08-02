import { ElementRef, EventEmitter } from '@angular/core';
import { Component, Input, OnInit, Output } from '@angular/core';
import { Track } from 'src/app/_models/track';
import { TrackLikeEvent } from '../../_events/trackLikeEvent';
import { TrackPlayEvent } from '../../_events/trackPlayEvent';

@Component({
	selector: 'app-track-item',
	templateUrl: './track-item.component.html',
	styleUrls: ['./track-item.component.css']
})
export class TrackItemComponent implements OnInit {
	@Input() track: Track;
	@Output() trackLiked = new EventEmitter<TrackLikeEvent>();
	@Output() trackPlayed = new EventEmitter<TrackPlayEvent>();
	@Output() trackExpanded = new EventEmitter<Track>();

	showDetail: boolean;

	constructor(public elementRef: ElementRef) { }

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

	expandTrackItem() {
		if (!this.showDetail) {
			this.trackExpanded.emit(this.track);
		}
	}

}
