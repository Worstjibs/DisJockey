import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { TracksService } from '../_services/tracks.service';
import { Track } from '../_models/track';
import { UserParams } from '../_models/userParams';
import { Pagination } from '../_models/pagination';

@Component({
    selector: 'app-tracks',
    templateUrl: './tracks.component.html',
    styleUrls: ['./tracks.component.css']
})
export class TracksComponent implements OnInit {
    // tracks$: Observable<Track[]>;
    tracks: Track[];
    userParams: UserParams;
    pagination: Pagination;

    constructor(private tracksService: TracksService) { 
        this.userParams = tracksService.getUserParams();
    }

    ngOnInit(): void {
        this.getTracks();
    }

    getTracks() {
        this.tracksService.getTracks(this.userParams).subscribe(response => {
            this.pagination = response.pagination;
            this.tracks = response.result;
        });
    }

    likeTrack(event) {
        var track: Track = event.track;

        if (event.liked != track.likedByUser) {
            this.tracksService.postTrackLike(event.track, event.liked);
        }
    }

    playTrack(event: Track) {
        if (event.youtubeId) {
            this.tracksService.playTrack(event);
        }
    }
}
