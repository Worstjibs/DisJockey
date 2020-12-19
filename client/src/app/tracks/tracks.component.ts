import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { TracksService } from '../_services/tracks.service';
import { HttpClient } from '@angular/common/http';

@Component({
    selector: 'app-tracks',
    templateUrl: './tracks.component.html',
    styleUrls: ['./tracks.component.css']
})
export class TracksComponent implements OnInit {
    tracks$: any;

    constructor(private tracksService: TracksService, private http: HttpClient) { }

    ngOnInit(): void {
        this.tracks$ = this.tracksService.getTracks();
    }
}
