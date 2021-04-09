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
    @Output() trackLiked = new EventEmitter<{track: Track, liked: boolean}>();

    constructor() { }

    ngOnInit(): void {
    }

    likeTrack(track: Track) {
        this.trackLiked.emit({track, liked: true});
    }

    dislikeTrack(track: Track) {
        this.trackLiked.emit({track, liked: false});
    }

}
