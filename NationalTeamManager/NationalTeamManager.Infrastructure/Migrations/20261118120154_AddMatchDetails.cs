using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NationalTeamManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Matches",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "AwayScoreDisplay",
                table: "Matches",
                type: "int",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "AwayScoreNormalTime",
                table: "Matches",
                type: "int",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "AwayScoreOvertime",
                table: "Matches",
                type: "int",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "AwayScorePenalties",
                table: "Matches",
                type: "int",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "AwayScorePeriod1",
                table: "Matches",
                type: "int",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "AwayScorePeriod2",
                table: "Matches",
                type: "int",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "CustomId",
                table: "Matches",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "HomeScoreDisplay",
                table: "Matches",
                type: "int",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "HomeScoreNormalTime",
                table: "Matches",
                type: "int",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "HomeScoreOvertime",
                table: "Matches",
                type: "int",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "HomeScorePenalties",
                table: "Matches",
                type: "int",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "HomeScorePeriod1",
                table: "Matches",
                type: "int",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "HomeScorePeriod2",
                table: "Matches",
                type: "int",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "RefereeId",
                table: "Matches",
                type: "int",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "StatusCode",
                table: "Matches",
                type: "int",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "StatusDescription",
                table: "Matches",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true
            );

            migrationBuilder.AddColumn<long>(
                name: "TimeCurrentPeriodStartTimestamp",
                table: "Matches",
                type: "bigint",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "TimeInjuryTime1",
                table: "Matches",
                type: "int",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "TimeInjuryTime2",
                table: "Matches",
                type: "int",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "TimeInjuryTime3",
                table: "Matches",
                type: "int",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "TimeInjuryTime4",
                table: "Matches",
                type: "int",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "TimeOvertimeLength",
                table: "Matches",
                type: "int",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "TimePeriodLength",
                table: "Matches",
                type: "int",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "TimeTotalPeriodCount",
                table: "Matches",
                type: "int",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "TournamentGroupName",
                table: "Matches",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "VenueId",
                table: "Matches",
                type: "int",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "WinnerCode",
                table: "Matches",
                type: "int",
                nullable: true
            );

            migrationBuilder.CreateTable(
                name: "Referees",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(
                        type: "nvarchar(150)",
                        maxLength: 150,
                        nullable: false
                    ),
                    Country = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                    ExternalId = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                    DataSource = table.Column<string>(
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Referees", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Venues",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(
                        type: "nvarchar(200)",
                        maxLength: 200,
                        nullable: false
                    ),
                    City = table.Column<string>(
                        type: "nvarchar(150)",
                        maxLength: 150,
                        nullable: true
                    ),
                    Country = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                    Capacity = table.Column<int>(type: "int", nullable: true),
                    Surface = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                    Latitude = table.Column<decimal>(
                        type: "decimal(9,6)",
                        precision: 9,
                        scale: 6,
                        nullable: true
                    ),
                    Longitude = table.Column<decimal>(
                        type: "decimal(9,6)",
                        precision: 9,
                        scale: 6,
                        nullable: true
                    ),
                    ExternalId = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                    DataSource = table.Column<string>(
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Venues", x => x.Id);
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Matches_RefereeId",
                table: "Matches",
                column: "RefereeId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Matches_VenueId",
                table: "Matches",
                column: "VenueId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Referees_ExternalId_DataSource",
                table: "Referees",
                columns: new[] { "ExternalId", "DataSource" },
                unique: true,
                filter: "[ExternalId] IS NOT NULL AND [DataSource] IS NOT NULL"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Venues_ExternalId_DataSource",
                table: "Venues",
                columns: new[] { "ExternalId", "DataSource" },
                unique: true,
                filter: "[ExternalId] IS NOT NULL AND [DataSource] IS NOT NULL"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Referees_RefereeId",
                table: "Matches",
                column: "RefereeId",
                principalTable: "Referees",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Venues_VenueId",
                table: "Matches",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Referees_RefereeId",
                table: "Matches"
            );

            migrationBuilder.DropForeignKey(name: "FK_Matches_Venues_VenueId", table: "Matches");

            migrationBuilder.DropTable(name: "Referees");

            migrationBuilder.DropTable(name: "Venues");

            migrationBuilder.DropIndex(name: "IX_Matches_RefereeId", table: "Matches");

            migrationBuilder.DropIndex(name: "IX_Matches_VenueId", table: "Matches");

            migrationBuilder.DropColumn(name: "AwayScoreDisplay", table: "Matches");

            migrationBuilder.DropColumn(name: "AwayScoreNormalTime", table: "Matches");

            migrationBuilder.DropColumn(name: "AwayScoreOvertime", table: "Matches");

            migrationBuilder.DropColumn(name: "AwayScorePenalties", table: "Matches");

            migrationBuilder.DropColumn(name: "AwayScorePeriod1", table: "Matches");

            migrationBuilder.DropColumn(name: "AwayScorePeriod2", table: "Matches");

            migrationBuilder.DropColumn(name: "CustomId", table: "Matches");

            migrationBuilder.DropColumn(name: "HomeScoreDisplay", table: "Matches");

            migrationBuilder.DropColumn(name: "HomeScoreNormalTime", table: "Matches");

            migrationBuilder.DropColumn(name: "HomeScoreOvertime", table: "Matches");

            migrationBuilder.DropColumn(name: "HomeScorePenalties", table: "Matches");

            migrationBuilder.DropColumn(name: "HomeScorePeriod1", table: "Matches");

            migrationBuilder.DropColumn(name: "HomeScorePeriod2", table: "Matches");

            migrationBuilder.DropColumn(name: "RefereeId", table: "Matches");

            migrationBuilder.DropColumn(name: "StatusCode", table: "Matches");

            migrationBuilder.DropColumn(name: "StatusDescription", table: "Matches");

            migrationBuilder.DropColumn(name: "TimeCurrentPeriodStartTimestamp", table: "Matches");

            migrationBuilder.DropColumn(name: "TimeInjuryTime1", table: "Matches");

            migrationBuilder.DropColumn(name: "TimeInjuryTime2", table: "Matches");

            migrationBuilder.DropColumn(name: "TimeInjuryTime3", table: "Matches");

            migrationBuilder.DropColumn(name: "TimeInjuryTime4", table: "Matches");

            migrationBuilder.DropColumn(name: "TimeOvertimeLength", table: "Matches");

            migrationBuilder.DropColumn(name: "TimePeriodLength", table: "Matches");

            migrationBuilder.DropColumn(name: "TimeTotalPeriodCount", table: "Matches");

            migrationBuilder.DropColumn(name: "TournamentGroupName", table: "Matches");

            migrationBuilder.DropColumn(name: "VenueId", table: "Matches");

            migrationBuilder.DropColumn(name: "WinnerCode", table: "Matches");

            migrationBuilder.DropColumn(name: "Slug", table: "Matches");
        }
    }
}
