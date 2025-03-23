using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialNetwork.Data.Migrations
{
    /// <inheritdoc />
    public partial class Song : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SelectedSong",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelectedSong",
                table: "AspNetUsers");
        }
    }
}
