import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { TimeagoModule } from 'ngx-timeago';
import { NgxSpinnerModule } from 'ngx-spinner';
import { YouTubePlayerModule } from '@angular/youtube-player';

@NgModule({
    declarations: [],
    imports: [
        CommonModule,
        TimeagoModule.forRoot(),
        BsDropdownModule.forRoot(),
        NgxSpinnerModule,
        YouTubePlayerModule
    ],
    exports: [
        TimeagoModule,
        BsDropdownModule,
        NgxSpinnerModule,
        YouTubePlayerModule
    ]
})
export class SharedModule { }
