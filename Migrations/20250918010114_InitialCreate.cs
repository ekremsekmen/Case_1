using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Case_1_2.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Stock = table.Column<int>(type: "integer", nullable: false),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "CreatedAt", "Description", "Name", "Price", "Stock", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "Electronics", new DateTime(2025, 9, 18, 1, 1, 14, 438, DateTimeKind.Utc).AddTicks(7090), "This is a sample product for testing", "Sample Product 1", 29.99m, 100, new DateTime(2025, 9, 18, 1, 1, 14, 438, DateTimeKind.Utc).AddTicks(7180) },
                    { 2, "Books", new DateTime(2025, 9, 18, 1, 1, 14, 438, DateTimeKind.Utc).AddTicks(7280), "Another sample product", "Sample Product 2", 49.99m, 50, new DateTime(2025, 9, 18, 1, 1, 14, 438, DateTimeKind.Utc).AddTicks(7280) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
