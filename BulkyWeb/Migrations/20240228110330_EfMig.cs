using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BulkyWeb.Migrations
{
    /// <inheritdoc />
    public partial class EfMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ISBN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ListPrice = table.Column<double>(type: "float", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Price50 = table.Column<double>(type: "float", nullable: false),
                    Price100 = table.Column<double>(type: "float", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "DisplayOrder", "Name" },
                values: new object[,]
                {
                    { 1, 9, "Biography" },
                    { 2, 9, "ScFi" },
                    { 3, 5, "Motivation" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Author", "CategoryId", "Description", "ISBN", "ImageUrl", "ListPrice", "Price", "Price100", "Price50", "Title" },
                values: new object[,]
                {
                    { 1, "Billy Spark", 1, "Praesent vitae sodales libero. Praesent molecule", "SWD9999001", "\\images\\Products\\pexels-ron-lach-9758163.jpg", 99.0, 90.0, 80.0, 85.0, "Fortune of Time" },
                    { 2, "David Goggins", 2, "For David Goggins, childhood was a nightmare -- poverty, prejudice, and physical abuse colored his days and haunted his nights", "SWD9999002", "\\images\\Products\\pexels-ron-lach-9758163.jpg", 9.9000000000000004, 9.0, 8.0, 8.5, "Can't hurt me" },
                    { 3, "Isayama Hajime", 3, "In a fictional world of terror , how can freedom be defined ? ", "SWD9999002", "\\images\\Products\\pexels-ron-lach-9758163.jpg", 59.0, 50.0, 40.0, 55.0, "Attack on Titan" },
                    { 4, "Thomas Pesquet", 2, "Dive into his fascination , the childhood dream , from the aircraf pilote to the nation hero", "SWD9999001", "\\images\\Products\\pexels-ron-lach-9758163.jpg", 29.0, 20.0, 10.0, 25.0, "Ma vie sans Gravite" },
                    { 5, "Jeremy Griffith", 2, "In the conspiracies theory , what's the meaning of life", "SWD9999005", "\\images\\Products\\pexels-ron-lach-9758163.jpg", 70.0, 79.0, 40.0, 55.0, "Freedom" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
