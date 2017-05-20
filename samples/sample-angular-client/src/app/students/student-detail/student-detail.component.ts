import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

import { Student } from '../../shared/models/student.model';
import { StudentType } from '../../shared/enums/student-type.enum';
import { StudentsService } from '../../shared/services/students.service';

@Component({
  selector: 'app-student-detail',
  templateUrl: './student-detail.component.html',
  styleUrls: ['./student-detail.component.scss']
})
export class StudentDetailComponent implements OnInit {

  student: Student = null;
  studentForm: FormGroup;

  formErrors = {
    'firstName': '',
    'lastName': ''
  };

  validationMessages = {
    'firstName': {
      'required': 'First name is required.',
      'minlength': 'First name must be at least 4 characters long.',
      'maxlength': 'First name cannot be more than 24 characters long.'
    },
    'lastName': {
      'required': 'Last name is required.'
    }
  };

  constructor(private route: ActivatedRoute, private studentService: StudentsService, private fb: FormBuilder) { }

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
    this.studentForm = this.fb.group({
      'firstName': [this.student.firstName, [
        Validators.required,
        Validators.minLength(4),
        Validators.maxLength(24)
      ]],
      'lastName': [this.student.lastName, Validators.required]
    });
    this.studentForm.valueChanges
      .subscribe(data => this.onValueChanged(data));
    this.onValueChanged(); // (re)set validation messages now
  }

  onValueChanged(data?: any) {
    if (!this.studentForm) { return; }
    const form = this.studentForm;
    for (const field in this.formErrors) {
      // clear previous error message (if any)
      this.formErrors[field] = '';
      const control = form.get(field);
      if (control && control.dirty && !control.valid) {
        const messages = this.validationMessages[field];
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
