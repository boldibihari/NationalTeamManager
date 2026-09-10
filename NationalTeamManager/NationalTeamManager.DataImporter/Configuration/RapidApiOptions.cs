namespace NationalTeamManager.DataImporter.Configuration
{
    public class RapidApiOptions
    {
        public const string SectionName = "RapidApi";

        public string BaseUrl { get; set; } = null!;
        public string Host { get; set; } = null!;
        public string ApiKey { get; set; } = null!;
    }
}
