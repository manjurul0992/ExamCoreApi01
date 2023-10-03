using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamCoreApi01.Migrations
{
    public partial class userSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "TblUsers",
                newName: "ID");

            migrationBuilder.InsertData(
                table: "TblUsers",
                columns: new[] { "ID", "Email", "FullName", "Password" },
                values: new object[] { 1, "manjurulislam327@gmail.com", "manjurul", "123456" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TblUsers",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "TblUsers",
                newName: "Id");
        }
    }
}
