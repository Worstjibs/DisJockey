import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';
import { AccountService } from '../_services/account.service';

@Component({
    selector: 'app-nav',
    templateUrl: './nav.component.html',
    styleUrls: ['./nav.component.css']
})
export class NavComponent {
    baseUrl: string;

    constructor(public accountService: AccountService) {
        this.baseUrl = environment.apiUrl;
    }

    logout() {
        this.accountService.logout();
    }

}
