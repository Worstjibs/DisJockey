import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NavComponent } from './nav/nav.component';
import { HomeComponent } from './home/home.component';
import { TracksComponent } from './tracks/tracks.component';
import { MembersComponent } from './members/members.component';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { LoadingInterceptor } from './_interceptors/loading.interceptor';
import { SharedModule } from './_modules/shared.module';
import { ErrorInterceptor } from './_interceptors/error.interceptor';
import { TrackItemComponent } from './tracks/track-item/track-item.component';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberPlaylistsComponent } from './members/member-playlists/member-playlists.component';
import { MemberCardComponent } from './members/member-card/member-card.component';
import { UnauthorizedComponent } from './errors/unauthorized/unauthorized.component';
import { TrackListComponent } from './tracks/track-list/track-list.component';
import { SearchComponent } from './search/search.component';


@NgModule({
    declarations: [
        AppComponent,
        NavComponent,
        HomeComponent,
        TracksComponent,
        MembersComponent,
        TrackItemComponent,
        NotFoundComponent,
        ServerErrorComponent,
        MemberDetailComponent,
        MemberPlaylistsComponent,
        MemberCardComponent,
        UnauthorizedComponent,
        TrackListComponent,
        SearchComponent
    ],
    imports: [
        BrowserModule,
        AppRoutingModule,
        HttpClientModule,
        BrowserAnimationsModule,
        SharedModule
    ],
    providers: [
        { provide : HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true },
        { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true }
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
