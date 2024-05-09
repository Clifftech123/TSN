using TertiarySchoolNavigator.Api.Models.SchoolModels;

namespace TertiarySchoolNavigator.Api.Contracts.School
{
    public class SchoolSearchResponse
    {
        public List<Schoolmodole> Schools { get; set; }
        public int TotalCount { get; set; }
    }
}
