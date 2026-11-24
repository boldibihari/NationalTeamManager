using System.Globalization;
using Microsoft.EntityFrameworkCore;
using NationalTeamManager.DataImporter.Models;
using NationalTeamManager.DataImporter.Providers.SofaScore;
using NationalTeamManager.DataImporter.Providers.SofaScore.Models;
using NationalTeamManager.Domain.Entities;
using NationalTeamManager.Infrastructure.Persistence;

namespace NationalTeamManager.DataImporter.Importers
{
    public class MatchDetailsImporter(
        ISofaScoreClient sofaScoreClient,
        IDbContextFactory<NationalTeamManagerDbContext> contextFactory
    )
    {
        private const string DataSource = "SofaScore";

        public async Task<MatchDetailsImportStatistics> ImportAsync(
            CancellationToken cancellationToken = default
        )
        {
            await using var readContext = await contextFactory.CreateDbContextAsync(
                cancellationToken
            );
            var matches = await readContext
                .Matches.AsNoTracking()
                .Include(x => x.Venue)
                .Where(x => x.DataSource == DataSource && x.ExternalId != null)
                .ToListAsync(cancellationToken);

            var statistics = new MatchDetailsImportStatistics();

            foreach (var match in matches)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!NeedsDetails(match))
                {
                    statistics.Matches.Add(ImportResult.Skipped);
                    continue;
                }

                if (
                    !int.TryParse(
                        match.ExternalId,
                        NumberStyles.None,
                        CultureInfo.InvariantCulture,
                        out var externalMatchId
                    )
                )
                {
                    statistics.Matches.Add(ImportResult.Skipped);
                    continue;
                }

                try
                {
                    var response = await sofaScoreClient.GetMatchDetailsAsync(
                        externalMatchId,
                        cancellationToken
                    );

                    if (response.Event is not { } details || details.Id != externalMatchId)
                    {
                        throw new InvalidDataException(
                            "A válaszból hiányzik a kért mérkőzés vagy eltér az azonosítója."
                        );
                    }

                    await SaveDetailsAsync(match.Id, details, statistics, cancellationToken);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (OperationCanceledException)
                {
                    statistics.Matches.Add(ImportResult.Failed);

                    Console.Error.WriteLine(
                        $"A mérkőzés részletes importja időtúllépés miatt sikertelen: {match.ExternalId}"
                    );
                }
                catch (Exception exception)
                {
                    statistics.Matches.Add(ImportResult.Failed);

                    Console.Error.WriteLine(
                        $"A mérkőzés részletes importja sikertelen ({match.ExternalId}): {exception.Message}"
                    );
                }
            }

            Console.WriteLine(
                $"""
                Részletes meccsimport kész:

                  Összesen: {matches.Count}
                  Mérkőzések:
                    Létrehozott: {statistics.Matches.Created}
                    Frissített: {statistics.Matches.Updated}
                    Változatlan: {statistics.Matches.Unchanged}
                    Kihagyott: {statistics.Matches.Skipped}
                    Hibás: {statistics.Matches.Failed}

                  Stadionok:
                    Létrehozott: {statistics.Venues.Created}
                    Frissített: {statistics.Venues.Updated}
                    Változatlan: {statistics.Venues.Unchanged}
                    Kihagyott: {statistics.Venues.Skipped}
                    Hibás: {statistics.Venues.Failed}

                  Játékvezetők:
                    Létrehozott: {statistics.Referees.Created}
                    Frissített: {statistics.Referees.Updated}
                    Változatlan: {statistics.Referees.Unchanged}
                    Kihagyott: {statistics.Referees.Skipped}
                    Hibás: {statistics.Referees.Failed}
                """
            );

            return statistics;
        }

        private static bool NeedsDetails(Match match)
        {
            var isLive =
                string.Equals(match.StatusType, "inprogress", StringComparison.OrdinalIgnoreCase)
                || string.Equals(match.StatusType, "live", StringComparison.OrdinalIgnoreCase);

            if (isLive || match.DetailsImportedAt is null)
            {
                return true;
            }

            var hasMissingVenueDetails =
                match.VenueId is null
                || match.Venue is null
                || string.IsNullOrWhiteSpace(match.Venue.Name)
                || string.IsNullOrWhiteSpace(match.Venue.City)
                || string.IsNullOrWhiteSpace(match.Venue.Country)
                || match.Venue.Capacity is null
                || string.IsNullOrWhiteSpace(match.Venue.Surface)
                || match.Venue.Latitude is null
                || match.Venue.Longitude is null;
            return hasMissingVenueDetails
                || match.RefereeId is null
                || match.TimePeriodLength is null
                || match.TimeOvertimeLength is null
                || match.TimeTotalPeriodCount is null;
        }

        private async Task SaveDetailsAsync(
            int matchId,
            SofaScoreMatchDetail source,
            MatchDetailsImportStatistics statistics,
            CancellationToken cancellationToken
        )
        {
            await using var dbContext = await contextFactory.CreateDbContextAsync(
                cancellationToken
            );
            var match = await dbContext.Matches.SingleAsync(
                x => x.Id == matchId,
                cancellationToken
            );
            var hasChanges = UpdateMatch(match, source);
            var (venue, venueResult) = await GetOrCreateVenueAsync(
                dbContext,
                source.Venue,
                cancellationToken
            );
            var (referee, refereeResult) = await GetOrCreateRefereeAsync(
                dbContext,
                source.Referee,
                cancellationToken
            );

            if (venue is not null)
            {
                hasChanges |=
                    match.VenueId != venue.Id
                    || venueResult is ImportResult.Created or ImportResult.Updated;
                match.Venue = venue;
            }
            if (referee is not null)
            {
                hasChanges |=
                    match.RefereeId != referee.Id
                    || refereeResult is ImportResult.Created or ImportResult.Updated;
                match.Referee = referee;
            }

            match.DetailsImportedAt = DateTime.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken);

            statistics.Matches.Add(hasChanges ? ImportResult.Updated : ImportResult.Unchanged);
            if (venueResult.HasValue)
                statistics.Venues.Add(venueResult.Value);
            if (refereeResult.HasValue)
                statistics.Referees.Add(refereeResult.Value);
        }

        private static bool UpdateMatch(Match match, SofaScoreMatchDetail source)
        {
            var hasChanges = false;

            hasChanges |= UpdateCurrentValue(match.Slug, source.Slug, value => match.Slug = value);
            hasChanges |= UpdateCurrentValue(
                match.CustomId,
                source.CustomId,
                value => match.CustomId = value
            );
            hasChanges |= UpdateCurrentValue(
                match.StatusType,
                source.Status?.Type,
                value => match.StatusType = value
            );
            hasChanges |= UpdateCurrentValue(
                match.StatusCode,
                source.Status?.Code,
                value => match.StatusCode = value
            );
            hasChanges |= UpdateCurrentValue(
                match.StatusDescription,
                source.Status?.Description,
                value => match.StatusDescription = value
            );
            hasChanges |= UpdateCurrentValue(
                match.WinnerCode,
                source.WinnerCode,
                value => match.WinnerCode = value
            );
            hasChanges |= UpdateCurrentValue(
                match.TournamentGroupName,
                source.Tournament?.GroupName,
                value => match.TournamentGroupName = value
            );

            if (source.StartTimestamp.HasValue)
            {
                var date = DateTimeOffset
                    .FromUnixTimeSeconds(source.StartTimestamp.Value)
                    .UtcDateTime;
                hasChanges |= SetIfDifferent(match.Date, date, value => match.Date = value);
            }

            hasChanges |= UpdateScore(
                match.HomeScore,
                source.HomeScore,
                value => match.HomeScore = value,
                value => match.HomeScoreDisplay = value,
                value => match.HomeScorePeriod1 = value,
                value => match.HomeScorePeriod2 = value,
                value => match.HomeScoreNormalTime = value,
                value => match.HomeScoreOvertime = value,
                value => match.HomeScorePenalties = value,
                match.HomeScoreDisplay,
                match.HomeScorePeriod1,
                match.HomeScorePeriod2,
                match.HomeScoreNormalTime,
                match.HomeScoreOvertime,
                match.HomeScorePenalties
            );
            hasChanges |= UpdateScore(
                match.AwayScore,
                source.AwayScore,
                value => match.AwayScore = value,
                value => match.AwayScoreDisplay = value,
                value => match.AwayScorePeriod1 = value,
                value => match.AwayScorePeriod2 = value,
                value => match.AwayScoreNormalTime = value,
                value => match.AwayScoreOvertime = value,
                value => match.AwayScorePenalties = value,
                match.AwayScoreDisplay,
                match.AwayScorePeriod1,
                match.AwayScorePeriod2,
                match.AwayScoreNormalTime,
                match.AwayScoreOvertime,
                match.AwayScorePenalties
            );

            hasChanges |= UpdateTime(match, source);
            return hasChanges;
        }

        private static async Task<(Venue? Entity, ImportResult? Result)> GetOrCreateVenueAsync(
            NationalTeamManagerDbContext dbContext,
            SofaScoreVenue? source,
            CancellationToken cancellationToken
        )
        {
            if (source?.Id is not int sourceId)
            {
                return (null, null);
            }

            var externalId = sourceId.ToString(CultureInfo.InvariantCulture);
            var venue = await dbContext.Venues.SingleOrDefaultAsync(
                x => x.DataSource == DataSource && x.ExternalId == externalId,
                cancellationToken
            );
            if (venue is not null)
            {
                var name = source.Name ?? source.Stadium?.Name;
                var city = source.City?.Name;
                var country = source.Country?.Name;
                var capacity = source.Capacity ?? source.Stadium?.Capacity;
                var surface =
                    source.SurfaceType ?? source.Stadium?.Surface ?? source.Stadium?.FieldType;
                var latitude =
                    source.Latitude
                    ?? source.VenueCoordinates?.Latitude
                    ?? source.Stadium?.Coordinates?.Latitude;
                var longitude =
                    source.Longitude
                    ?? source.VenueCoordinates?.Longitude
                    ?? source.Stadium?.Coordinates?.Longitude;
                var hasChanges = false;

                hasChanges |= UpdateSupplementalValue(
                    venue.Name,
                    name,
                    value => venue.Name = value
                );
                hasChanges |= UpdateSupplementalValue(
                    venue.City,
                    city,
                    value => venue.City = value
                );
                hasChanges |= UpdateSupplementalValue(
                    venue.Country,
                    country,
                    value => venue.Country = value
                );
                hasChanges |= UpdateSupplementalValue(
                    venue.Capacity,
                    capacity,
                    value => venue.Capacity = value
                );
                hasChanges |= UpdateSupplementalValue(
                    venue.Surface,
                    surface,
                    value => venue.Surface = value
                );
                hasChanges |= UpdateSupplementalValue(
                    venue.Latitude,
                    latitude,
                    value => venue.Latitude = value
                );
                hasChanges |= UpdateSupplementalValue(
                    venue.Longitude,
                    longitude,
                    value => venue.Longitude = value
                );

                return (venue, hasChanges ? ImportResult.Updated : ImportResult.Unchanged);
            }

            var newVenueName = source.Name ?? source.Stadium?.Name;
            if (string.IsNullOrWhiteSpace(newVenueName))
            {
                return (null, ImportResult.Skipped);
            }

            venue = new Venue
            {
                Name = newVenueName,
                City = source.City?.Name,
                Country = source.Country?.Name,
                Capacity = source.Capacity ?? source.Stadium?.Capacity,
                Surface =
                    source.SurfaceType ?? source.Stadium?.Surface ?? source.Stadium?.FieldType,
                Latitude =
                    source.Latitude
                    ?? source.VenueCoordinates?.Latitude
                    ?? source.Stadium?.Coordinates?.Latitude,
                Longitude =
                    source.Longitude
                    ?? source.VenueCoordinates?.Longitude
                    ?? source.Stadium?.Coordinates?.Longitude,
                ExternalId = externalId,
                DataSource = DataSource,
            };

            dbContext.Venues.Add(venue);
            return (venue, ImportResult.Created);
        }

        private static async Task<(Referee? Entity, ImportResult? Result)> GetOrCreateRefereeAsync(
            NationalTeamManagerDbContext dbContext,
            SofaScoreReferee? source,
            CancellationToken cancellationToken
        )
        {
            if (source?.Id is not int sourceId)
            {
                return (null, null);
            }

            var externalId = sourceId.ToString(CultureInfo.InvariantCulture);
            var referee = await dbContext.Referees.SingleOrDefaultAsync(
                x => x.DataSource == DataSource && x.ExternalId == externalId,
                cancellationToken
            );
            if (referee is not null)
            {
                var hasChanges = UpdateSupplementalValue(
                    referee.Name,
                    source.Name,
                    value => referee.Name = value
                );
                hasChanges |= UpdateSupplementalValue(
                    referee.Country,
                    source.Country?.Name,
                    value => referee.Country = value
                );
                return (referee, hasChanges ? ImportResult.Updated : ImportResult.Unchanged);
            }

            if (string.IsNullOrWhiteSpace(source.Name))
            {
                return (null, ImportResult.Skipped);
            }

            referee = new Referee
            {
                Name = source.Name,
                Country = source.Country?.Name,
                ExternalId = externalId,
                DataSource = DataSource,
            };

            dbContext.Referees.Add(referee);
            return (referee, ImportResult.Created);
        }

        private static bool UpdateTime(Match match, SofaScoreMatchDetail details)
        {
            var source = details.Time;

            var hasChanges = false;
            hasChanges |= UpdateCurrentValue(
                match.TimePeriodLength,
                details.DefaultPeriodLength ?? source?.DefaultPeriodLength ?? source?.PeriodLength,
                value => match.TimePeriodLength = value
            );
            hasChanges |= UpdateCurrentValue(
                match.TimeOvertimeLength,
                details.DefaultOvertimeLength
                    ?? source?.DefaultOvertimeLength
                    ?? source?.OvertimeLength,
                value => match.TimeOvertimeLength = value
            );
            hasChanges |= UpdateCurrentValue(
                match.TimeTotalPeriodCount,
                details.DefaultPeriodCount
                    ?? source?.DefaultPeriodCount
                    ?? source?.TotalPeriodCount,
                value => match.TimeTotalPeriodCount = value
            );
            hasChanges |= UpdateCurrentValue(
                match.TimeCurrentPeriodStartTimestamp,
                source?.CurrentPeriodStartTimestamp,
                value => match.TimeCurrentPeriodStartTimestamp = value
            );
            hasChanges |= UpdateCurrentValue(
                match.TimeInjuryTime1,
                source?.InjuryTime1,
                value => match.TimeInjuryTime1 = value
            );
            hasChanges |= UpdateCurrentValue(
                match.TimeInjuryTime2,
                source?.InjuryTime2,
                value => match.TimeInjuryTime2 = value
            );
            hasChanges |= UpdateCurrentValue(
                match.TimeInjuryTime3,
                source?.InjuryTime3,
                value => match.TimeInjuryTime3 = value
            );
            hasChanges |= UpdateCurrentValue(
                match.TimeInjuryTime4,
                source?.InjuryTime4,
                value => match.TimeInjuryTime4 = value
            );
            return hasChanges;
        }

        private static bool UpdateScore(
            int? current,
            SofaScoreScore? source,
            Action<int?> setCurrent,
            Action<int?> setDisplay,
            Action<int?> setPeriod1,
            Action<int?> setPeriod2,
            Action<int?> setNormalTime,
            Action<int?> setOvertime,
            Action<int?> setPenalties,
            int? display,
            int? period1,
            int? period2,
            int? normalTime,
            int? overtime,
            int? penalties
        )
        {
            if (source is null)
                return false;

            var hasChanges = false;
            hasChanges |= UpdateCurrentValue(current, source.Current, setCurrent);
            hasChanges |= UpdateCurrentValue(display, source.Display, setDisplay);
            hasChanges |= UpdateCurrentValue(period1, source.Period1, setPeriod1);
            hasChanges |= UpdateCurrentValue(period2, source.Period2, setPeriod2);
            hasChanges |= UpdateCurrentValue(normalTime, source.NormalTime, setNormalTime);
            hasChanges |= UpdateCurrentValue(overtime, source.Overtime, setOvertime);
            hasChanges |= UpdateCurrentValue(penalties, source.Penalties, setPenalties);
            return hasChanges;
        }

        private static bool UpdateCurrentValue<T>(T? current, T? value, Action<T?> setter)
            where T : struct
        {
            if (
                !value.HasValue
                || current.HasValue
                    && EqualityComparer<T>.Default.Equals(current.Value, value.Value)
            )
                return false;

            setter(value.Value);
            return true;
        }

        private static bool UpdateCurrentValue(
            string? current,
            string? value,
            Action<string> setter
        )
        {
            if (
                string.IsNullOrWhiteSpace(value)
                || string.Equals(current, value, StringComparison.Ordinal)
            )
                return false;

            setter(value);
            return true;
        }

        private static bool UpdateSupplementalValue<T>(T? current, T? value, Action<T?> setter)
            where T : struct => UpdateCurrentValue(current, value, setter);

        private static bool UpdateSupplementalValue(
            string? current,
            string? value,
            Action<string> setter
        ) => UpdateCurrentValue(current, value, setter);

        private static bool SetIfDifferent<T>(T current, T value, Action<T> setter)
            where T : IEquatable<T>
        {
            if (current.Equals(value))
                return false;

            setter(value);
            return true;
        }
    }
}
