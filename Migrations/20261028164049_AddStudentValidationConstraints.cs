using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentCourseEFCore.Migrations
{
    public partial class AddStudentValidationConstraints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Restrict the Name column to 100 characters.
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Students",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // Restrict the Email column to 450 characters.
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Students",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // Require email addresses to be unique.
            migrationBuilder.CreateIndex(
                name: "IX_Students_Email",
                table: "Students",
                column: "Email",
                unique: true);

            // Enforce the permitted age range in SQL Server.
            migrationBuilder.AddCheckConstraint(
                name: "CK_Students_Age",
                table: "Students",
                sql: "[Age] BETWEEN 17 AND 60");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove the age constraint.
            migrationBuilder.DropCheckConstraint(
                name: "CK_Students_Age",
                table: "Students");

            // Remove the unique email index.
            migrationBuilder.DropIndex(
                name: "IX_Students_Email",
                table: "Students");

            // Restore the original Email column definition.
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450);

            // Restore the original Name column definition.
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}