import { TrackUser } from "./trackUser";

export interface Track {
    youtubeId: string,
    users: TrackUser[],
    createdOn: Date,
    likes: Number,
    dislikes: Number,
    title: string,
    channelTitle: string,
    description: string,
    smallThumbnail: string,
    mediumThumbnail: string,
    largeThumbnail: string,
    clicked: boolean
}