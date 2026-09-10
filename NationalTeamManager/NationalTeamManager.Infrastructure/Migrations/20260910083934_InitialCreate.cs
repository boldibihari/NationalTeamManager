using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NationalTeamManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clubs",
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
                    table.PrimaryKey("PK_Clubs", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Competitions",
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
                    table.PrimaryKey("PK_Competitions", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    Position = table.Column<int>(type: "int", nullable: false),
                    Height = table.Column<int>(type: "int", nullable: true),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: true),
                    PreferredFoot = table.Column<int>(type: "int", nullable: false),
                    Nationality = table.Column<string>(
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
                    ClubId = table.Column<int>(type: "int", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Clubs_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Clubs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Opponent = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    IsHome = table.Column<bool>(type: "bit", nullable: false),
                    HungaryScore = table.Column<int>(type: "int", nullable: true),
                    OpponentScore = table.Column<int>(type: "int", nullable: true),
                    Venue = table.Column<string>(
                        type: "nvarchar(200)",
                        maxLength: 200,
                        nullable: true
                    ),
                    CompetitionId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_Matches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matches_Competitions_CompetitionId",
                        column: x => x.CompetitionId,
                        principalTable: "Competitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "PlayerMarketValues",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(
                        type: "decimal(18,2)",
                        precision: 18,
                        scale: 2,
                        nullable: false
                    ),
                    Currency = table.Column<string>(
                        type: "nvarchar(3)",
                        maxLength: 3,
                        nullable: false
                    ),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerMarketValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerMarketValues_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Clubs_ExternalId_DataSource",
                table: "Clubs",
                columns: new[] { "ExternalId", "DataSource" },
                unique: true,
                filter: "[ExternalId] IS NOT NULL AND [DataSource] IS NOT NULL"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Competitions_ExternalId_DataSource",
                table: "Competitions",
                columns: new[] { "ExternalId", "DataSource" },
                unique: true,
                filter: "[ExternalId] IS NOT NULL AND [DataSource] IS NOT NULL"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Matches_CompetitionId",
                table: "Matches",
                column: "CompetitionId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Matches_ExternalId_DataSource",
                table: "Matches",
                columns: new[] { "ExternalId", "DataSource" },
                unique: true,
                filter: "[ExternalId] IS NOT NULL AND [DataSource] IS NOT NULL"
            );

            migrationBuilder.CreateIndex(
                name: "IX_PlayerMarketValues_PlayerId_RecordedAt",
                table: "PlayerMarketValues",
                columns: new[] { "PlayerId", "RecordedAt" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Players_ClubId",
                table: "Players",
                column: "ClubId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Players_ExternalId_DataSource",
                table: "Players",
                columns: new[] { "ExternalId", "DataSource" },
                unique: true,
                filter: "[ExternalId] IS NOT NULL AND [DataSource] IS NOT NULL"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Matches");

            migrationBuilder.DropTable(name: "PlayerMarketValues");

            migrationBuilder.DropTable(name: "Competitions");

            migrationBuilder.DropTable(name: "Players");

            migrationBuilder.DropTable(name: "Clubs");
        }
    }
}
