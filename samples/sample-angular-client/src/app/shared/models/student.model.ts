import { Person } from "./person.model";
import { StudentType } from "../enums/student-type.enum";

export class Student extends Person {
    yearOfGraduation: Date;
    type: StudentType;
}
