using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NationalTeamManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamsAndCompetitionStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Competitions_CompetitionId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Clubs_ClubId",
                table: "Players");

            migrationBuilder.DropTable(
                name: "Clubs");

            migrationBuilder.DropColumn(
                name: "IsHome",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "Opponent",
                table: "Matches");

            migrationBuilder.RenameColumn(
                name: "ClubId",
                table: "Players",
                newName: "TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_Players_ClubId",
                table: "Players",
                newName: "IX_Players_TeamId");

            migrationBuilder.RenameColumn(
                name: "OpponentScore",
                table: "Matches",
                newName: "HomeScore");

            migrationBuilder.RenameColumn(
                name: "HungaryScore",
                table: "Matches",
                newName: "CompetitionStageId");

            migrationBuilder.RenameColumn(
                name: "CompetitionId",
                table: "Matches",
                newName: "HomeTeamId");

            migrationBuilder.RenameIndex(
                name: "IX_Matches_CompetitionId",
                table: "Matches",
                newName: "IX_Matches_HomeTeamId");

            migrationBuilder.AddColumn<int>(
                name: "AwayScore",
                table: "Matches",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AwayTeamId",
                table: "Matches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompetitionEditionId",
                table: "Matches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CompetitionEditions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Year = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CompetitionId = table.Column<int>(type: "int", nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DataSource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompetitionEditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompetitionEditions_Competitions_CompetitionId",
                        column: x => x.CompetitionId,
                        principalTable: "Competitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsNationalTeam = table.Column<bool>(type: "bit", nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DataSource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompetitionStages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CompetitionEditionId = table.Column<int>(type: "int", nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DataSource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompetitionStages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompetitionStages_CompetitionEditions_CompetitionEditionId",
                        column: x => x.CompetitionEditionId,
                        principalTable: "CompetitionEditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_AwayTeamId",
                table: "Matches",
                column: "AwayTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_CompetitionEditionId",
                table: "Matches",
                column: "CompetitionEditionId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_CompetitionStageId",
                table: "Matches",
                column: "CompetitionStageId");

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionEditions_CompetitionId",
                table: "CompetitionEditions",
                column: "CompetitionId");

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionEditions_ExternalId_DataSource",
                table: "CompetitionEditions",
                columns: new[] { "ExternalId", "DataSource" },
                unique: true,
                filter: "[ExternalId] IS NOT NULL AND [DataSource] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionStages_CompetitionEditionId",
                table: "CompetitionStages",
                column: "CompetitionEditionId");

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionStages_ExternalId_DataSource",
                table: "CompetitionStages",
                columns: new[] { "ExternalId", "DataSource" },
                unique: true,
                filter: "[ExternalId] IS NOT NULL AND [DataSource] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_ExternalId_DataSource",
                table: "Teams",
                columns: new[] { "ExternalId", "DataSource" },
                unique: true,
                filter: "[ExternalId] IS NOT NULL AND [DataSource] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_CompetitionEditions_CompetitionEditionId",
                table: "Matches",
                column: "CompetitionEditionId",
                principalTable: "CompetitionEditions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_CompetitionStages_CompetitionStageId",
                table: "Matches",
                column: "CompetitionStageId",
                principalTable: "CompetitionStages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Teams_AwayTeamId",
                table: "Matches",
                column: "AwayTeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Teams_HomeTeamId",
                table: "Matches",
                column: "HomeTeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Teams_TeamId",
                table: "Players",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_CompetitionEditions_CompetitionEditionId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_CompetitionStages_CompetitionStageId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Teams_AwayTeamId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Teams_HomeTeamId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Teams_TeamId",
                table: "Players");

            migrationBuilder.DropTable(
                name: "CompetitionStages");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "CompetitionEditions");

            migrationBuilder.DropIndex(
                name: "IX_Matches_AwayTeamId",
                table: "Matches");

            migrationBuilder.DropIndex(
                name: "IX_Matches_CompetitionEditionId",
                table: "Matches");

            migrationBuilder.DropIndex(
                name: "IX_Matches_CompetitionStageId",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "AwayScore",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "AwayTeamId",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "CompetitionEditionId",
                table: "Matches");

            migrationBuilder.RenameColumn(
                name: "TeamId",
                table: "Players",
                newName: "ClubId");

            migrationBuilder.RenameIndex(
                name: "IX_Players_TeamId",
                table: "Players",
                newName: "IX_Players_ClubId");

            migrationBuilder.RenameColumn(
                name: "HomeTeamId",
                table: "Matches",
                newName: "CompetitionId");

            migrationBuilder.RenameColumn(
                name: "HomeScore",
                table: "Matches",
                newName: "OpponentScore");

            migrationBuilder.RenameColumn(
                name: "CompetitionStageId",
                table: "Matches",
                newName: "HungaryScore");

            migrationBuilder.RenameIndex(
                name: "IX_Matches_HomeTeamId",
                table: "Matches",
                newName: "IX_Matches_CompetitionId");

            migrationBuilder.AddColumn<bool>(
                name: "IsHome",
                table: "Matches",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Opponent",
                table: "Matches",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Clubs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DataSource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ExternalId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clubs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clubs_ExternalId_DataSource",
                table: "Clubs",
                columns: new[] { "ExternalId", "DataSource" },
                unique: true,
                filter: "[ExternalId] IS NOT NULL AND [DataSource] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Competitions_CompetitionId",
                table: "Matches",
                column: "CompetitionId",
                principalTable: "Competitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Clubs_ClubId",
                table: "Players",
                column: "ClubId",
                principalTable: "Clubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
