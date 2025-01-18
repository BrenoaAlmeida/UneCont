using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Model.Migrations
{
    public partial class Melhoriasnasclassesdemodelo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Arquivo",
                table: "LogArquivo");

            migrationBuilder.DropColumn(
                name: "DataHoraInsercao",
                table: "LogArquivo");

            migrationBuilder.DropColumn(
                name: "DataHoraInsercao",
                table: "LogAgora");

            migrationBuilder.AlterColumn<string>(
                name: "TipoLog",
                table: "LogArquivo",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CaminhoDoArquivo",
                table: "LogArquivo",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaminhoDoArquivo",
                table: "LogArquivo");

            migrationBuilder.AlterColumn<string>(
                name: "TipoLog",
                table: "LogArquivo",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 20);

            migrationBuilder.AddColumn<byte[]>(
                name: "Arquivo",
                table: "LogArquivo",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataHoraInsercao",
                table: "LogArquivo",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DataHoraInsercao",
                table: "LogAgora",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
