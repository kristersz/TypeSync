import { FormGroup, FormBuilder, Validators } from '@angular/forms';

import { Student } from '../../shared/models/student.model';

export class StudentValidator {
  public validationMessages = {
    'firstName': {
      'required': 'First name is required.',
      'minlength': 'First name must be at least 4 characters long.',
      'maxlength': 'First name cannot be more than 24 characters long.'
    },
    'lastName': {
      'required': 'Last name is required.'
    }
  };

  constructor(public fb: FormBuilder) { }

  buildForm(student: Student): FormGroup {
    return this.fb.group({
      'firstName': [student.firstName, [
        Validators.required,
        Validators.minLength(4),
        Validators.maxLength(24)
      ]],
      'lastName': [student.lastName, Validators.required]
    });
  }
}
