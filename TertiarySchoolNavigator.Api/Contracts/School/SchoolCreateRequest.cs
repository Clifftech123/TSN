namespace TertiarySchoolNavigator.Api.Contracts.School
{
    public class SchoolCreateRequest
    {

        public string Name { get; set; }
        public string Region { get; set; }
        public string District { get; set; }
        public int EstablishedYear { get; set; }
        public string SchoolType { get; set; }
    }
}
