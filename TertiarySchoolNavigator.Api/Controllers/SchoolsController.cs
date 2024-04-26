using Microsoft.AspNetCore.Mvc;
using TertiarySchoolNavigator.Api.Interface;
using TertiarySchoolNavigator.Api.Models.SchoolModels;
using TertiarySchoolNavigator.Api.Models.SchoolModels.TertiarySchoolNavigator.Api.Models.SchoolModels;

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



        // Create a new school
        [HttpPost]
        public async Task<ActionResult<SchoolResponse>> CreateSchool([FromBody] SchoolCreateRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Region) || string.IsNullOrEmpty(request.District))
            {
                return BadRequest("School fields cannot be empty and must be provided");
            }

            var school = await _schoolService.CreateSchool(request);

            if (school == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating new school record");
            }

            var response = new SchoolResponse
            {
                School = school,
                Message = "School created successfully"
            };

            return CreatedAtAction(nameof(GetSchoolById), new { id = school.Id }, response);
        }




        // Get a school by id

        [HttpGet("{id}")]
        public async Task<ActionResult<School>> GetSchoolById(int id)
        {
            try
            {
                var school = await _schoolService.GetSchoolById(id);
                if (school == null)
                {
                    return NotFound("School not found");
                }

                return Ok(school);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }



        //  Update an existing school
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




        // Search for schools by name, region, district, or established year
        [HttpPost("search")]
        public async Task<ActionResult<List<School>>> SearchSchools(SchoolSearchRequest request)
        {
            try
            {
                var schools = await _schoolService.SearchSchools(request);
                if (schools == null || !schools.Any())
                {
                    return NotFound("No schools found matching the search criteria");
                }

                var response = new SchoolSearchResponse
                {
                    Schools = schools,
                    TotalCount = schools.Count
                };

                return Ok(response);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }


        //  Get all schools
        [HttpGet]
        public async Task<ActionResult<IEnumerable<School>>> GetAllSchools()
        {
            var schools = await _schoolService.GetAllSchools();
            return Ok(schools);
        }



        // Delete a school

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
