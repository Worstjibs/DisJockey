import { Component, OnInit } from '@angular/core';
import { environment } from '../../environments/environment.prod';

@Component({
	selector: 'app-home',
	templateUrl: './home.component.html',
	styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
	title: string;

	constructor() { }

	ngOnInit(): void {
		this.title = environment.title;
	}

}
