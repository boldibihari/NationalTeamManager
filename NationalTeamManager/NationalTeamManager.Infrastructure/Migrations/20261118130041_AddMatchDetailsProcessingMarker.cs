using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NationalTeamManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchDetailsProcessingMarker : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DetailsImportedAt",
                table: "Matches",
                type: "datetime2",
                nullable: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "DetailsImportedAt", table: "Matches");
        }
    }
}
