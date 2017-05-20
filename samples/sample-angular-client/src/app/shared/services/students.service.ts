import { Injectable } from '@angular/core';
import { Http, Headers, Response } from '@angular/http';
import 'rxjs/add/operator/toPromise';
import { Student } from '../models/student.model';

@Injectable()
export class StudentsService {
    private baseUrl = 'api/students';
    constructor(private http: Http) { }
    list(): Promise<Student[]> {
        return this.http.get(this.baseUrl + '/').toPromise().then((response: Response) => response.json()).catch(this.handleError);
    }
    get(id: number): Promise<Student> {
        return this.http.get(this.baseUrl + ('/' + id)).toPromise().then((response: Response) => response.json()).catch(this.handleError);
    }
    post(student: Student): Promise<number> {
        return this.http.post(this.baseUrl, student).toPromise().then((response: Response) => response.json()).catch(this.handleError);
    }
    put(id: number, student: Student): Promise<void> {
        return this.http.put(this.baseUrl + ('/' + id), student).toPromise().then((response: Response) => response.json()).catch(this.handleError);
    }
    delete(id: number): Promise<void> {
        return this.http.delete(this.baseUrl + ('/' + id)).toPromise().then((response: Response) => response.json()).catch(this.handleError);
    }
    handleError(error: any): Promise<any> {
        return Promise.reject(error.message || error);
    }
}
