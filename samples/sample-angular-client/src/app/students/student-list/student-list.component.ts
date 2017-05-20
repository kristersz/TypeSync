import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { Student } from '../../shared/models/student.model';
import { StudentType } from '../../shared/enums/student-type.enum';
import { StudentsService } from '../../shared/services/students.service';

@Component({
  selector: 'app-student-list',
  templateUrl: './student-list.component.html',
  styleUrls: ['./student-list.component.scss']
})
export class StudentListComponent implements OnInit {

  students: Student[] = [];

  StudentType = StudentType;

  constructor(private studentService: StudentsService, private router: Router) { }

  ngOnInit() {
    this.studentService.list().then(students => this.students = students);
  }

  editStudent(id: number) {
    this.router.navigate(['/student', id]);
  }
}
