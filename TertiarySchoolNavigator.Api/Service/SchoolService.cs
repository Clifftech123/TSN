
using System.Text.Json;
using TertiarySchoolNavigator.Api.Interface;
using TertiarySchoolNavigator.Api.Models.SchoolModels;

namespace TertiarySchoolNavigator.Api.Service
{
    public class SchoolService : ISchoolService
    {
        private readonly string _jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SchoolData.json");

        private async Task<List<SchoolData>> LoadSchoolDataAsync()
        {
            using FileStream openStream = File.OpenRead(_jsonFilePath);
            var schoolData = await JsonSerializer.DeserializeAsync<List<SchoolData>>(openStream);
            return schoolData ?? new List<SchoolData>();
        }

        public async Task<List<SchoolData>> GetAllSchools()
        {
            try
            {
                return await LoadSchoolDataAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading school data: {ex.Message}");
                return new List<SchoolData>(); // Return an empty list in case of error
            }
        }

        public async Task<List<SchoolData>> SearchSchools(string name, string location, string nickname)
        {
            try
            {
                var schools = await LoadSchoolDataAsync();
                return schools.Where(s =>
                    (!string.IsNullOrEmpty(name) && s.Name.Contains(name, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(nickname) && s.Nickname.Contains(nickname, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(location) && s.Location.Any(l => l.Contains(location, StringComparison.OrdinalIgnoreCase))) ||
                    (!string.IsNullOrEmpty(s.Region) && s.Region.Contains(location, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(s.District) && s.District.Contains(location, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching school data: {ex.Message}");
                return new List<SchoolData>(); // Return an empty list in case of error
            }
        }
    }
}