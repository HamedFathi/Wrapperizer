using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wrapperizer.Abstraction.Cqrs;
using Wrapperizer.Sample.Domain.Commands;

namespace Wrapperizer.Sample.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class StudentController : Controller
    {
        private readonly ICommandQueryManager _manager;

        public StudentController(ICommandQueryManager manager)
        {
            _manager = manager;
        }

        [HttpGet]
        public IActionResult GetStudentInfo(Guid studentId) => Ok();

        [HttpPost("register")]
        public async Task<IActionResult> RegisterStudent([FromBody]RegisterStudent command)
        {
            var studentId = await _manager.Send(command);
            return CreatedAtAction(nameof(GetStudentInfo), new {studentId});
        }
    }
}