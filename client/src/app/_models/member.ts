import { Track } from "./track";

export interface Member {
    username: string;
    avatarUrl: string;
    tracks: Track[];
    dateJoined: Date;
}