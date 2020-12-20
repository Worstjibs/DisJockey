import { User } from "./user";

export interface Track {
    youtubeId: string,
    users: User[],
    createdOn: Date,
    likes: Number,
    dislikes: Number
}