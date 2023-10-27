import { Component, OnInit, ViewChild } from '@angular/core';
import { TrackListComponent } from '../tracks/track-list/track-list.component';
import { TrackListType } from '../_enums/trackListType';

@Component({
    selector: 'app-search',
    templateUrl: './search.component.html',
    styleUrls: ['./search.component.css']
})
export class SearchComponent implements OnInit {
    @ViewChild('trackListComponent') trackListComponent: TrackListComponent;

    TrackListType = TrackListType;

    constructor() { }

    ngOnInit(): void {
    }

    search(query: string) {
        this.trackListComponent.search(query);
    }
}
