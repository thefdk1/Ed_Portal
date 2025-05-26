using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Internet_Programciligi.Migrations
{
    /// <inheritdoc />
    public partial class mig8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "TeacherProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TeacherProfiles_CategoryId",
                table: "TeacherProfiles",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherProfiles_Categories_CategoryId",
                table: "TeacherProfiles",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeacherProfiles_Categories_CategoryId",
                table: "TeacherProfiles");

            migrationBuilder.DropIndex(
                name: "IX_TeacherProfiles_CategoryId",
                table: "TeacherProfiles");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "TeacherProfiles");
        }
    }
}
