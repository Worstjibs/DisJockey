import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { NgxSpinnerModule } from 'ngx-spinner';
import { YouTubePlayerModule } from '@angular/youtube-player';
import { ToastrModule } from 'ngx-toastr';
import { FormsModule } from '@angular/forms';

@NgModule({
    declarations: [],
    imports: [
        CommonModule,
        TimeagoModule.forRoot(),
        BsDropdownModule.forRoot(),
        NgxSpinnerModule,
        YouTubePlayerModule,
        ToastrModule.forRoot({
            positionClass: 'toast-bottom-right'
        }),
        PaginationModule.forRoot(),
        TabsModule.forRoot(),
        FormsModule
    ],
    exports: [
        TimeagoModule,
        BsDropdownModule,
        NgxSpinnerModule,
        YouTubePlayerModule,
        ToastrModule,
        PaginationModule,
        TabsModule,
        FormsModule
    ]
})
export class SharedModule { }
