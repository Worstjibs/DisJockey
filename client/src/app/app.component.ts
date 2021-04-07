import { OnInit } from '@angular/core';
import { Component } from '@angular/core';
import { AccountService } from './_services/account.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
    title = 'DisJockey';

    constructor(private accountService: AccountService) {

    }

    ngOnInit() {
        this.accountService.getClaims();
    }
}
