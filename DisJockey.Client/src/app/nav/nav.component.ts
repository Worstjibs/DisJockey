import { Component, OnInit } from '@angular/core';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { CurrentUser } from '../_models/currentUser';
import { AccountService } from '../_services/account.service';

@Component({
    selector: 'app-nav',
    templateUrl: './nav.component.html',
    styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
    baseUrl: string;

    constructor(public accountService: AccountService) {
    }

    ngOnInit(): void {
        this.baseUrl = environment.apiUrl;
    }

    logout() {
        this.accountService.logout();
    }

}
