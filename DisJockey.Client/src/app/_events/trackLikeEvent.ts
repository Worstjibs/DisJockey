import { Track } from "../_models/track";

export interface TrackLikeEvent {
	track: Track;
	liked: boolean;
}