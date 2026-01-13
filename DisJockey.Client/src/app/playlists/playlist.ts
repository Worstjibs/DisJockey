import { Track } from "../tracks/models/track";

export interface Playlist {
	name: string;
	youtubeId: string;
	tracks: Track[];
}