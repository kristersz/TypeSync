import { Address } from "./address.model";
import { Gender } from "../enums/gender.enum";

export class Person {
    id: number;
    firstName: string;
    lastName: string;
    dateOfBirth: Date;
    address: Address;
    gender: Gender;
}
