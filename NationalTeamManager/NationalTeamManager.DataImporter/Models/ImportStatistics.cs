namespace NationalTeamManager.DataImporter.Models
{
    public class ImportStatistics
    {
        public int Created { get; private set; }

        public int Updated { get; private set; }

        public int Unchanged { get; private set; }

        public int Skipped { get; private set; }

        public int Failed { get; private set; }

        public void Add(ImportResult result)
        {
            switch (result)
            {
                case ImportResult.Created:
                    Created++;
                    break;

                case ImportResult.Updated:
                    Updated++;
                    break;

                case ImportResult.Unchanged:
                    Unchanged++;
                    break;

                case ImportResult.Skipped:
                    Skipped++;
                    break;

                case ImportResult.Failed:
                    Failed++;
                    break;
            }
        }
    }
}
