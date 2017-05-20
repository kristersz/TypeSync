using System;
using System.Collections.Generic;
using Samples.DotNetFull.ViewModels;

namespace Samples.DotNetFull.WebAPI.Services
{
    public class StudentService : IStudentService
    {
        public List<Student> GetStudents()
        {
            return new List<Student>()
            {
                new Student()
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Wick",
                    DateOfBirth = new DateTime(1980, 1, 1),
                    YearOfGraduation = DateTime.Now,
                    Type = StudentType.Domestic
                },
                new Student()
                {
                    Id = 2,
                    FirstName = "Molly",
                    LastName = "Wilkins",
                    DateOfBirth = new DateTime(1989, 1, 1),
                    YearOfGraduation = DateTime.Now.AddYears(-9),
                    Type = StudentType.Foreign
                }
            };
        }

        public Student GetStudent(long id)
        {
            return new Student()
            {
                Id = 1,
                FirstName = "John",
                LastName = "Wick",
                DateOfBirth = new DateTime(1980, 1, 1),
                YearOfGraduation = DateTime.Now,
                Type = StudentType.Domestic
            };
        }
    }
}