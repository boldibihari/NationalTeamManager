using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NationalTeamManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchIncidents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MatchIncidents",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MatchId = table.Column<int>(type: "int", nullable: false),
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
                    IncidentType = table.Column<string>(
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: true
                    ),
                    IncidentClass = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                    Time = table.Column<int>(type: "int", nullable: true),
                    AddedTime = table.Column<int>(type: "int", nullable: true),
                    ReversedPeriodTime = table.Column<int>(type: "int", nullable: true),
                    IsHome = table.Column<bool>(type: "bit", nullable: true),
                    PlayerExternalId = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                    PlayerName = table.Column<string>(
                        type: "nvarchar(150)",
                        maxLength: 150,
                        nullable: true
                    ),
                    AssistPlayerExternalId = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                    AssistPlayerName = table.Column<string>(
                        type: "nvarchar(150)",
                        maxLength: 150,
                        nullable: true
                    ),
                    PlayerInExternalId = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                    PlayerInName = table.Column<string>(
                        type: "nvarchar(150)",
                        maxLength: 150,
                        nullable: true
                    ),
                    PlayerOutExternalId = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                    PlayerOutName = table.Column<string>(
                        type: "nvarchar(150)",
                        maxLength: 150,
                        nullable: true
                    ),
                    HomeScore = table.Column<int>(type: "int", nullable: true),
                    AwayScore = table.Column<int>(type: "int", nullable: true),
                    Reason = table.Column<string>(
                        type: "nvarchar(200)",
                        maxLength: 200,
                        nullable: true
                    ),
                    Rescinded = table.Column<bool>(type: "bit", nullable: true),
                    Text = table.Column<string>(
                        type: "nvarchar(500)",
                        maxLength: 500,
                        nullable: true
                    ),
                    Description = table.Column<string>(
                        type: "nvarchar(1000)",
                        maxLength: 1000,
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchIncidents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchIncidents_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_MatchIncidents_ExternalId_DataSource",
                table: "MatchIncidents",
                columns: new[] { "ExternalId", "DataSource" },
                unique: true,
                filter: "[ExternalId] IS NOT NULL AND [DataSource] IS NOT NULL"
            );

            migrationBuilder.CreateIndex(
                name: "IX_MatchIncidents_MatchId",
                table: "MatchIncidents",
                column: "MatchId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "MatchIncidents");
        }
    }
}
