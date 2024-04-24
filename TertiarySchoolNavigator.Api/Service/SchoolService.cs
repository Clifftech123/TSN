using Microsoft.EntityFrameworkCore;
using TertiarySchoolNavigator.Api.Domain;
using TertiarySchoolNavigator.Api.Interface;
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

        public async Task<School> CreateSchool(SchoolCreateRequest request)
        {
            var school = new School
            {
                Name = request.Name,
                Region = request.Region,
                District = request.District,
                EstablishedYear = request.EstablishedYear,
                // Add other properties here
            };

            _context.Schools.Add(school);
            await _context.SaveChangesAsync();

            return school;
        }



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
            // Update other properties here

            await _context.SaveChangesAsync();

            return school;
        }


        public async Task<List<School>> GetAllSchools()
        {
            return await _context.Schools.ToListAsync();
        }

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


            return await query.ToListAsync();
        }


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


    }
}
