import { Track } from "../_models/track";

export interface TrackPlayEvent {
	track: Track;
	playNow: boolean;
}