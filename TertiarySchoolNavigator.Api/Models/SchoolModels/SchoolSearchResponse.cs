namespace TertiarySchoolNavigator.Api.Models.SchoolModels
{
    public class SchoolSearchResponse
    {
        public List<School> Schools { get; set; }
        public int TotalCount { get; set; }
    }
}
