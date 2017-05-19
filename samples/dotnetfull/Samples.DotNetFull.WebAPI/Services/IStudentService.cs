using System.Collections.Generic;
using Samples.DotNetFull.ViewModels;

namespace Samples.DotNetFull.WebAPI.Services
{
    public interface IStudentService
    {
        List<Student> GetStudents();

        Student GetStudent(long id);
    }
}
