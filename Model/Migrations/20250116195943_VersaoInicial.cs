using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Model.Migrations
{
    public partial class VersaoInicial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Log",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Url = table.Column<string>(nullable: true),
                    DataDeInsercao = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LogAgora",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    HttpMethod = table.Column<string>(maxLength: 20, nullable: false),
                    StatusCode = table.Column<string>(maxLength: 20, nullable: false),
                    UriPath = table.Column<string>(maxLength: 100, nullable: false),
                    TimeTaken = table.Column<int>(nullable: false),
                    ResponseSize = table.Column<string>(nullable: false),
                    CacheStatus = table.Column<string>(maxLength: 50, nullable: false),
                    LogId = table.Column<int>(nullable: false),
                    DataHoraInsercao = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogAgora", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogAgora_Log_LogId",
                        column: x => x.LogId,
                        principalTable: "Log",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LogArquivo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LogId = table.Column<int>(nullable: false),
                    TipoLog = table.Column<string>(nullable: true),
                    NomeArquivo = table.Column<string>(maxLength: 100, nullable: false),
                    Arquivo = table.Column<byte[]>(nullable: true),
                    DataHoraInsercao = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogArquivo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogArquivo_Log_LogId",
                        column: x => x.LogId,
                        principalTable: "Log",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LogMinhaCdn",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ResponseSize = table.Column<string>(maxLength: 20, nullable: false),
                    StatusCode = table.Column<string>(maxLength: 10, nullable: false),
                    CacheStatus = table.Column<string>(maxLength: 30, nullable: false),
                    Request = table.Column<string>(maxLength: 50, nullable: false),
                    TimeTaken = table.Column<string>(nullable: false),
                    LogId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogMinhaCdn", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogMinhaCdn_Log_LogId",
                        column: x => x.LogId,
                        principalTable: "Log",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LogAgora_LogId",
                table: "LogAgora",
                column: "LogId");

            migrationBuilder.CreateIndex(
                name: "IX_LogArquivo_LogId",
                table: "LogArquivo",
                column: "LogId");

            migrationBuilder.CreateIndex(
                name: "IX_LogMinhaCdn_LogId",
                table: "LogMinhaCdn",
                column: "LogId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogAgora");

            migrationBuilder.DropTable(
                name: "LogArquivo");

            migrationBuilder.DropTable(
                name: "LogMinhaCdn");

            migrationBuilder.DropTable(
                name: "Log");
        }
    }
}
