using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoInter.Migrations
{
    /// <inheritdoc />
    public partial class RemovendoTrocarSenhaUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "precisatrocarsenha",
                table: "funcionario");

            migrationBuilder.AlterColumn<DateTime>(
                name: "data",
                table: "atendimentoveterinario",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "precisatrocarsenha",
                table: "funcionario",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "data",
                table: "atendimentoveterinario",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);
        }
    }
}
