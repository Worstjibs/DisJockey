import { Playlist } from "./playlist";
import { Track } from "./track";

export interface Member {
    username: string;
    avatarUrl: string;
    dateJoined: Date;
    discordId: string;
    tracksPlayed: number;
    tracks: Track[];
    playlists: Playlist[];
}