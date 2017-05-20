import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule, Routes } from '@angular/router';

import { AppComponent } from './app.component';
import { StudentListComponent } from './students/student-list/student-list.component';
import { StudentDetailComponent } from './students/student-detail/student-detail.component';

import { StudentsService } from './shared/services/students.service';

const appRoutes: Routes = [
  { path: 'student', component: StudentDetailComponent },
  { path: 'student/:id', component: StudentDetailComponent },
  {
    path: 'students',
    component: StudentListComponent,
    data: { title: 'Student List' }
  },
  {
    path: '',
    redirectTo: '/students',
    pathMatch: 'full'
  },
  // { path: '**', component: PageNotFoundComponent }
];

@NgModule({
  declarations: [
    AppComponent,
    StudentListComponent,
    StudentDetailComponent
  ],
  imports: [
    BrowserModule,
    ReactiveFormsModule,
    HttpModule,
    RouterModule.forRoot(appRoutes)
  ],
  providers: [StudentsService],
  bootstrap: [AppComponent]
})
export class AppModule { }
