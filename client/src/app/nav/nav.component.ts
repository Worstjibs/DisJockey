import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';

@Component({
    selector: 'app-nav',
    templateUrl: './nav.component.html',
    styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
    baseUrl: string

    constructor() { }

    ngOnInit(): void {
        this.baseUrl = environment.apiUrl;
    }

}
