using Microsoft.EntityFrameworkCore;
using TertiarySchoolNavigator.Api.Domain;
using TertiarySchoolNavigator.Api.Interface;
using TertiarySchoolNavigator.Api.Middleware;
using TertiarySchoolNavigator.Api.Models.SchoolModels;

namespace TertiarySchoolNavigator.Api.Service
{
    public class SchoolService : ISchoolService


    {
        // Inject AppDbContext
        private readonly AppDbContext _context;

        // Constructor for SchoolService
        public SchoolService(AppDbContext context)
        {
            _context = context;
        }


        // Create a new school
        public async Task<School> CreateSchool(SchoolCreateRequest request)
        {
            var school = new School
            {
                Name = request.Name,
                Region = request.Region,
                District = request.District,
                EstablishedYear = request.EstablishedYear,
                SchoolType = request.SchoolType,

              
            };

            _context.Schools.Add(school);
            await _context.SaveChangesAsync();

            return school;
        }



        // Update an existing school
        public async Task<School> UpdateSchool(SchoolUpdateRequest request)
        {
            var school = await _context.Schools.FindAsync(request.Id);
            if (school == null)
            {
                // Handle the case where the school doesn't exist
                return null;
            }

            school.Name = request.Name;
            school.Region = request.Region;
            school.District = request.District;
            school.EstablishedYear = request.EstablishedYear;
            school.SchoolType = request.SchoolType;

            await _context.SaveChangesAsync();

            return school;
        }





        // Get all schools
        public async Task<List<School>> GetAllSchools()
        {
            var schools = await _context.Schools.ToListAsync();
            if (!schools.Any())
            {
                throw new Exception("No schools found");
            }
            return schools;
        }


        // Search for schools by name, region, district, or established year
        public async Task<List<School>> SearchSchools(SchoolSearchRequest request)
        {
            var query = _context.Schools.AsQueryable();

            if (!string.IsNullOrEmpty(request.Name))
            {
                query = query.Where(s => s.Name.Contains(request.Name));
            }

            if (!string.IsNullOrEmpty(request.Region))
            {
                query = query.Where(s => s.Region == request.Region);
            }

            if (!string.IsNullOrEmpty(request.District))
            {
                query = query.Where(s => s.District == request.District);
            }

            if (request.EstablishedYear.HasValue)
            {
                query = query.Where(s => s.EstablishedYear == request.EstablishedYear.Value);
            }

            var schools = await query.ToListAsync();

            if (!schools.Any())
            {
                throw new Exception("No schools found matching the search criteria");
            }

            return schools;
        }



        // Delete a school
        public async Task<bool> DeleteSchool(int id)
        {
            var school = await _context.Schools.FindAsync(id);
            if (school == null)
            {

                return false;
            }

            _context.Schools.Remove(school);
            await _context.SaveChangesAsync();

            return true;
        }


        // Get a school by id
        public async Task<School> GetSchoolById(int id)
        {
            var school = await _context.Schools.FindAsync(id);
            if (school == null)
            {
                throw new Exception("School not found");
            }

            return school;
        }

    }
}
