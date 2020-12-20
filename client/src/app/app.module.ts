import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NavComponent } from './nav/nav.component';
import { HomeComponent } from './home/home.component';
import { TracksComponent } from './tracks/tracks.component';
import { UsersComponent } from './users/users.component';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { YouTubePlayerModule } from '@angular/youtube-player';
import { NgxSpinnerModule } from 'ngx-spinner';
import { LoadingInterceptor } from './_interceptors/loading.interceptor';

@NgModule({
    declarations: [
        AppComponent,
        NavComponent,
        HomeComponent,
        TracksComponent,
        UsersComponent
    ],
    imports: [
        BrowserModule,
        AppRoutingModule,
        HttpClientModule,
        YouTubePlayerModule,
        BrowserAnimationsModule,
        NgxSpinnerModule
    ],
    providers: [
        { provide : HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true }
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
