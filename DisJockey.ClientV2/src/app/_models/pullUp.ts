import { BaseTrack } from "./baseTrack";

export interface PullUp extends BaseTrack {
	lastPulled: Date;
}