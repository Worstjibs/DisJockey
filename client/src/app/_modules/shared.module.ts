import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { PaginationModule } from 'ngx-bootstrap/pagination'
import { TimeagoModule } from 'ngx-timeago';
import { NgxSpinnerModule } from 'ngx-spinner';
import { YouTubePlayerModule } from '@angular/youtube-player';
import { ToastrModule } from 'ngx-toastr';
import { NgxPaginationModule } from 'ngx-pagination';

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
        PaginationModule.forRoot()
    ],
    exports: [
        TimeagoModule,
        BsDropdownModule,
        NgxSpinnerModule,
        YouTubePlayerModule,
        ToastrModule,
        PaginationModule
    ]
})
export class SharedModule { }
