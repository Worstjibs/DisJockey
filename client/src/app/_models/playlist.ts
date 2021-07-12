import { Track } from "./track";

export interface Playlist {
	name: string;
	youtubeId: string;
	tracks: Track[];
}