namespace TertiarySchoolNavigator.Api.Models.SchoolModels
{
    public class School
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Region { get; set; }
        public string District { get; set; }
        public int EstablishedYear { get; set; }
        public string SchoolType { get; set; }
    }
}
