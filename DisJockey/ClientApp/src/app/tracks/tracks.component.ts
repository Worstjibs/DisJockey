import { AfterViewChecked, Component, ElementRef, HostListener, OnInit, ViewChild, ViewChildren } from '@angular/core';
import { Observable } from 'rxjs';
import { TracksService } from '../_services/tracks.service';
import { Track } from '../_models/track';
import { UserParams } from '../_models/userParams';
import { TrackListComponent } from './track-list/track-list.component';
import { TrackListType } from '../_enums/trackListType';

@Component({
	selector: 'app-tracks',
	templateUrl: './tracks.component.html',
	styleUrls: ['./tracks.component.css']
})
export class TracksComponent {
	@ViewChild(TrackListComponent) trackListComponent: TrackListComponent;

	TrackListType = TrackListType;

	constructor() {   
	}

	sortBy(predicate: string) {
		this.trackListComponent.sortBy(predicate);
	}
}
