using TertiarySchoolNavigator.Api.Contracts.School;
using TertiarySchoolNavigator.Api.Models.SchoolModels;

namespace TertiarySchoolNavigator.Api.Interface
{
    public interface ISchoolService
    {
        Task<Schoolmodole> CreateSchool(SchoolCreateRequest request);
        Task<Schoolmodole> UpdateSchool(SchoolUpdateRequest request);
        Task<List<Schoolmodole>> SearchSchools(SchoolSearchRequest request);
        Task<List<Schoolmodole>> GetAllSchools();
        Task<bool> DeleteSchool(int id);

        Task<Schoolmodole> GetSchoolById(int id);
    }
}
