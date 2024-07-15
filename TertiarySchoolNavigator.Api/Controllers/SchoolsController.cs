using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TertiarySchoolNavigator.Api.Interface;
using TertiarySchoolNavigator.Api.Models.SchoolModels;

namespace TertiarySchoolNavigator.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolsController : ControllerBase
    {
        private readonly ISchoolService _schoolService;

        public SchoolsController(ISchoolService schoolService)
        {
            _schoolService = schoolService;
        }

        // Get all schools
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SchoolData>>> GetAllSchools()
        {
            var schools = await _schoolService.GetAllSchools();
            if (schools == null || !schools.Any())
            {
                return NotFound("No schools found");
            }
            return Ok(schools);
        }

        // Search for schools by name, location, or nickname
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<SchoolData>>> SearchSchools([FromQuery] string name, [FromQuery] string location, [FromQuery] string nickname)
        {
            var schools = await _schoolService.SearchSchools(name, location, nickname);
            if (schools == null || !schools.Any())
            {
                return NotFound("No schools found matching the search criteria");
            }
            return Ok(schools);
        }
    }
}