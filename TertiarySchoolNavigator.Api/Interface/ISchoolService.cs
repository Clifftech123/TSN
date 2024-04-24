using TertiarySchoolNavigator.Api.Models.SchoolModels;

namespace TertiarySchoolNavigator.Api.Interface
{
    public interface ISchoolService
    {
        Task<School> CreateSchool(SchoolCreateRequest request);
        Task<School> UpdateSchool(SchoolUpdateRequest request);
        Task<List<School>> SearchSchools(SchoolSearchRequest request);
        Task<List<School>> GetAllSchools();
        Task<bool> DeleteSchool(int id);
    }
}
