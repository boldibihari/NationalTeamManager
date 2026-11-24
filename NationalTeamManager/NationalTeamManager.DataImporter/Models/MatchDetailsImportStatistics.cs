namespace NationalTeamManager.DataImporter.Models
{
    public class MatchDetailsImportStatistics
    {
        public ImportStatistics Matches { get; } = new();

        public ImportStatistics Venues { get; } = new();

        public ImportStatistics Referees { get; } = new();
    }
}
