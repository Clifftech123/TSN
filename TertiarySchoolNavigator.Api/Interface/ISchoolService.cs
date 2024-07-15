
using TertiarySchoolNavigator.Api.Models.SchoolModels;

namespace TertiarySchoolNavigator.Api.Interface
{
    public interface ISchoolService
    {
    
        Task<List<SchoolData>> GetAllSchools();
        Task<List<SchoolData>> SearchSchools(string name, string location, string nickname);
    }
}
