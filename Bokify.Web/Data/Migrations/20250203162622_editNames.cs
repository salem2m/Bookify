using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bokify.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class editNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastUpdetedOn",
                table: "Categories",
                newName: "LastUpdatedOn");

            migrationBuilder.RenameColumn(
                name: "publishingDate",
                table: "Books",
                newName: "PublishingDate");

            migrationBuilder.RenameColumn(
                name: "publisher",
                table: "Books",
                newName: "Publisher");

            migrationBuilder.RenameColumn(
                name: "LastUpdetedOn",
                table: "Books",
                newName: "LastUpdatedOn");

            migrationBuilder.RenameColumn(
                name: "LastUpdetedOn",
                table: "Authors",
                newName: "LastUpdatedOn");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastUpdatedOn",
                table: "Categories",
                newName: "LastUpdetedOn");

            migrationBuilder.RenameColumn(
                name: "PublishingDate",
                table: "Books",
                newName: "publishingDate");

            migrationBuilder.RenameColumn(
                name: "Publisher",
                table: "Books",
                newName: "publisher");

            migrationBuilder.RenameColumn(
                name: "LastUpdatedOn",
                table: "Books",
                newName: "LastUpdetedOn");

            migrationBuilder.RenameColumn(
                name: "LastUpdatedOn",
                table: "Authors",
                newName: "LastUpdetedOn");
        }
    }
}
