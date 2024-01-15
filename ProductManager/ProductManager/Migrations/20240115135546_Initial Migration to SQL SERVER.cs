using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductManager.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigrationtoSQLSERVER : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CompanyPhoneNumber",
                table: "Company",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "CompanyName",
                table: "Company",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "CompanyEmail",
                table: "Company",
                newName: "Email");

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "Company",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Company");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Company",
                newName: "CompanyPhoneNumber");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Company",
                newName: "CompanyName");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Company",
                newName: "CompanyEmail");
        }
    }
}
