using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NationalTeamManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchIncidentExternalKeyAndMatchStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MatchIncidents_ExternalId_DataSource",
                table: "MatchIncidents"
            );

            migrationBuilder.AddColumn<string>(
                name: "ExternalKey",
                table: "MatchIncidents",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true
            );

            migrationBuilder.Sql(
                """
                UPDATE [MatchIncidents]
                SET [ExternalKey] = CASE
                    WHEN [ExternalId] IS NOT NULL THEN CONCAT(N'id:', [ExternalId])
                    ELSE CONCAT(N'legacy:', CONVERT(nvarchar(20), [Id]))
                END
                WHERE [ExternalKey] IS NULL;
                """
            );

            migrationBuilder.AlterColumn<string>(
                name: "ExternalKey",
                table: "MatchIncidents",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "StatusType",
                table: "Matches",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_MatchIncidents_DataSource_ExternalKey",
                table: "MatchIncidents",
                columns: new[] { "DataSource", "ExternalKey" },
                unique: true,
                filter: "[DataSource] IS NOT NULL"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MatchIncidents_DataSource_ExternalKey",
                table: "MatchIncidents"
            );

            migrationBuilder.DropColumn(name: "ExternalKey", table: "MatchIncidents");

            migrationBuilder.DropColumn(name: "StatusType", table: "Matches");

            migrationBuilder.CreateIndex(
                name: "IX_MatchIncidents_ExternalId_DataSource",
                table: "MatchIncidents",
                columns: new[] { "ExternalId", "DataSource" },
                unique: true,
                filter: "[ExternalId] IS NOT NULL AND [DataSource] IS NOT NULL"
            );
        }
    }
}
