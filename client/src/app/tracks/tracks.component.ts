import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { TracksService } from '../_services/tracks.service';
import { Track } from '../_models/track';

@Component({
    selector: 'app-tracks',
    templateUrl: './tracks.component.html',
    styleUrls: ['./tracks.component.css']
})
export class TracksComponent implements OnInit {
    tracks$: Observable<Track[]>;

    constructor(private tracksService: TracksService) { }

    ngOnInit(): void {
        this.tracks$ = this.tracksService.getTracks();
    }
}