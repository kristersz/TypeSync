using System.Collections.Generic;
using System.Web.Http;
using Samples.DotNetFull.ViewModels;
using Samples.DotNetFull.WebAPI.Services;

namespace Samples.DotNetFull.WebAPI.Controllers
{
    [RoutePrefix("api/students")]
    public class StudentsController : ApiController
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        // GET: api/students
        [HttpGet]
        public IEnumerable<Student> List()
        {
            return _studentService.GetStudents();
        }

        // GET api/students/5
        [HttpGet]
        [Route("{id}")]
        public Student Get(long id)
        {
            return _studentService.GetStudent(id);
        }

        // POST api/students
        [HttpPost]
        public void Post([FromBody]Student student)
        {
            // persist
        }

        // PUT api/students/5
        [HttpPut]
        [Route("{id}")]
        public void Put(long id, [FromBody]Student student)
        {
            // persist
        }

        // DELETE api/students/5
        [HttpDelete]
        [Route("{id}")]
        public void Delete(long id)
        {
            // delete from store
        }

        [HttpGet]
        [Route("types")]
        public List<KeyValuePair<int, string>> GetStudentTypes()
        {
            return new List<KeyValuePair<int, string>>()
            {
                new KeyValuePair<int, string>((int)StudentType.Domestic, StudentType.Domestic.ToString()),
                new KeyValuePair<int, string>((int)StudentType.Exchange, StudentType.Exchange.ToString()),
                new KeyValuePair<int, string>((int)StudentType.Foreign, StudentType.Foreign.ToString())
            };
        }

        [HttpGet]
        [Route("summary")]
        public IHttpActionResult Summary()
        {
            return Ok();
        }
    }
}