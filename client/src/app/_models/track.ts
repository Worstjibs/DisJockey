import { TrackUser } from "./trackUser";

export interface Track {
    youtubeId: string;
    users?: TrackUser[];
    createdOn: Date;
    likes: number;
    dislikes: number;
    likedByUser?: boolean;
    title: string;
    channelTitle: string;
    description: string;
    smallThumbnail: string;
    mediumThumbnail: string;
    largeThumbnail: string;
}