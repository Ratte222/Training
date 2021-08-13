using BLL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Training.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExternLibController : ControllerBase
    {
        private readonly IPythonLibService _pythonLibService;

        public ExternLibController(IPythonLibService pythonLibService)
        {
            _pythonLibService = pythonLibService;
        }

        [HttpPost("CalcArrSum")]
        public IActionResult CalcArrSum(int[] array)
        {
            return Ok(_pythonLibService.CalcArraySum(array));
        }
    }
}
