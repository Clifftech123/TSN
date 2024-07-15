namespace TertiarySchoolNavigator.Api.Models.SchoolModels;

public partial class SchoolData
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Region { get; set; }
    public string District { get; set; }
    public int EstablishedYear { get; set; }
    public string SchoolType { get; set; }
    public string Nickname { get; set; }
    public int? Founded { get; set; }
    public string Undergrad { get; set; }
    public string Postgrad { get; set; }
    public int? Total2011 { get; set; }
    public List<string> Location { get; set; }
}
