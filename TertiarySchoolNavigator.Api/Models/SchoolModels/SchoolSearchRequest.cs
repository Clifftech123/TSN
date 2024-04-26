namespace TertiarySchoolNavigator.Api.Models.SchoolModels
{
    public class SchoolSearchRequest
    {
        public string Name { get; set; }
        public string Region { get; set; }
        public string District { get; set; }
        public int? EstablishedYear { get; set; }
    }
}
