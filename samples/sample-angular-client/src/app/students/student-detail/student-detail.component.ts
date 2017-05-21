import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

import { Student } from '../../shared/models/student.model';
import { StudentType } from '../../shared/enums/student-type.enum';
import { StudentsService } from '../../shared/services/students.service';

import { StudentValidator } from '../../shared/validators/student.validator';

@Component({
  selector: 'app-student-detail',
  templateUrl: './student-detail.component.html',
  styleUrls: ['./student-detail.component.scss']
})
export class StudentDetailComponent implements OnInit {

  student: Student = null;
  studentForm: FormGroup;
  studentValidator: StudentValidator;

  formErrors = {
    'firstName': '',
    'lastName': ''
  };

  constructor(private route: ActivatedRoute, private studentService: StudentsService, private fb: FormBuilder) {
    this.studentValidator = new StudentValidator(this.fb);
  }

  ngOnInit() {
    const id = +this.route.snapshot.params['id'];

    if (id) {
      this.studentService.get(id).then(student => {
        this.student = student;
      });
    } else {
      this.student = {
        id: 0,
        firstName: '',
        lastName: '',
        dateOfBirth: null,
        address: null,
        gender: null,
        yearOfGraduation: null,
        type: null
      };
      this.buildForm();
    }
  }

  buildForm(): void {
    this.studentForm = this.studentValidator.buildForm(this.student);

    this.studentForm.valueChanges.subscribe(data => this.onValueChanged(data));
    this.onValueChanged(); // (re)set validation messages now
  }

  onValueChanged(data?: any) {
    if (!this.studentForm) { return; }
    const form = this.studentForm;
    // tslint:disable-next-line:forin
    for (const field in this.formErrors) {
      // clear previous error message (if any)
      this.formErrors[field] = '';

      const control = form.get(field);
      if (control && control.dirty && !control.valid) {
        const messages = this.studentValidator.validationMessages[field];
        // tslint:disable-next-line:forin
        for (const key in control.errors) {
          this.formErrors[field] += messages[key] + ' ';
        }
      }
    }
  }

  createNew() {
    if (this.studentForm.invalid) {
      alert('Client says model is invalid');
      return;
    }

    this.studentService.post(this.student).then(id => {
      if (id === 0) {
        alert('Server says model is invalid!');
      } else {
        this.student.id = id;

        alert('Student was successfully created!');
      }
    });
  }
}
