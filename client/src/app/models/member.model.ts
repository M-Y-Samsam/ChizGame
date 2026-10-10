import { Photo } from "./photo.model";

export interface Member {
    name: string;
    age: number;
    gender: string;
    country: string;
    city: string;
    photo: Photo | null;
    lastActive: Date;
}