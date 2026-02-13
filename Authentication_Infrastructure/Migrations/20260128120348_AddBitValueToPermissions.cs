using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Authentication_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBitValueToPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BitValue",
                table: "Permissions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BitValue",
                table: "Permissions");
        }
    }
}
