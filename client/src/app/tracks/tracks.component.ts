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
    tracks: Track[];
    userParams: UserParams;
    pagination: Pagination;

    constructor(private tracksService: TracksService) {
        this.userParams = tracksService.resetUserParams();
        console.log(this.userParams);
    }

    ngOnInit(): void {
        this.getTracks();
    }
    
    getTracks(): void {
        this.tracks = [];
        this.userParams = new UserParams();

        this.tracksService.getTracks(this.userParams).subscribe(response => {
            this.pagination = response.pagination;
            this.tracks = response.result;
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

    loadMore(): void {
        this.userParams.pageNumber++;

        this.tracksService.getTracks(this.userParams).subscribe(response => {
            this.tracks = this.tracks.concat(response.result);
        });
    }
}
