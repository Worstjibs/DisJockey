import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { TimeagoModule } from 'ngx-timeago';
import { NgxSpinnerModule } from 'ngx-spinner';
import { YouTubePlayerModule } from '@angular/youtube-player';
import { ToastrModule } from 'ngx-toastr';

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
        })
    ],
    exports: [
        TimeagoModule,
        BsDropdownModule,
        NgxSpinnerModule,
        YouTubePlayerModule,
        ToastrModule
    ]
})
export class SharedModule { }
