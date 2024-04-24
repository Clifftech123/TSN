using Microsoft.AspNetCore.Mvc;
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



        [HttpPost]
        public async Task<ActionResult<School>> CreateSchool(SchoolCreateRequest request)
        {
            var school = await _schoolService.CreateSchool(request);
            return CreatedAtAction(nameof(GetSchool), new { id = school.Id }, school);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSchool(int id, SchoolUpdateRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }

            var school = await _schoolService.UpdateSchool(request);
            if (school == null)
            {
                return NotFound();
            }

            return NoContent();
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<School>>> GetAllSchools()
        {
            var schools = await _schoolService.GetAllSchools();
            return Ok(schools);
        }




        [HttpGet("{id}")]
        public async Task<ActionResult<School>> GetSchool(int id)
        {
            var school = await _schoolService.GetSchool(id);
            if (school == null)
            {
                return NotFound();
            }

            return Ok(school);
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSchool(int id)
        {
            var result = await _schoolService.DeleteSchool(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }




    }
}
