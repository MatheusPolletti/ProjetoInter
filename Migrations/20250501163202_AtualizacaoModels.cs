using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoInter.Migrations
{
    /// <inheritdoc />
    public partial class AtualizacaoModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Setores",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Observacoes",
                table: "Procedimentos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Instituicoes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Funcionarios",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "AtendimentosVeterinarios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateOnly>(
                name: "DataFalecimento",
                table: "Animais",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Animais",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Setores");

            migrationBuilder.DropColumn(
                name: "Observacoes",
                table: "Procedimentos");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Instituicoes");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Funcionarios");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AtendimentosVeterinarios");

            migrationBuilder.DropColumn(
                name: "DataFalecimento",
                table: "Animais");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Animais");
        }
    }
}
