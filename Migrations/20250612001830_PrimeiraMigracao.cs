using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace ProjetoInter.Migrations
{
    /// <inheritdoc />
    public partial class PrimeiraMigracao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "animalespecie",
                columns: table => new
                {
                    animalespecieid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    descricao = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_animalespecie", x => x.animalespecieid);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "animalstatus",
                columns: table => new
                {
                    animalstatusid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    descricao = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_animalstatus", x => x.animalstatusid);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "instituicao",
                columns: table => new
                {
                    instituicaoid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "longtext", nullable: false),
                    endereco = table.Column<string>(type: "longtext", nullable: false),
                    contato = table.Column<string>(type: "longtext", nullable: true),
                    imagemurl = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_instituicao", x => x.instituicaoid);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "statusfuncionario",
                columns: table => new
                {
                    statusfuncionarioid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    descricao = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_statusfuncionario", x => x.statusfuncionarioid);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "setor",
                columns: table => new
                {
                    setorid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    status = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    instituicaopertence = table.Column<int>(type: "int", nullable: false),
                    nome = table.Column<string>(type: "longtext", nullable: false),
                    descricao = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_setor", x => x.setorid);
                    table.ForeignKey(
                        name: "FK_setor_instituicao_instituicaopertence",
                        column: x => x.instituicaopertence,
                        principalTable: "instituicao",
                        principalColumn: "instituicaoid",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "funcionario",
                columns: table => new
                {
                    funcionarioid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    auth_user_id = table.Column<Guid>(type: "char(36)", nullable: true),
                    status = table.Column<int>(type: "int", nullable: false),
                    nome = table.Column<string>(type: "longtext", nullable: false),
                    cpf = table.Column<string>(type: "longtext", nullable: false),
                    cargo = table.Column<string>(type: "longtext", nullable: false),
                    telefone = table.Column<string>(type: "longtext", nullable: true),
                    imagemurl = table.Column<string>(type: "longtext", nullable: true),
                    isadmin = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    precisatrocarsenha = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    instituicaoid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_funcionario", x => x.funcionarioid);
                    table.ForeignKey(
                        name: "FK_funcionario_instituicao_instituicaoid",
                        column: x => x.instituicaoid,
                        principalTable: "instituicao",
                        principalColumn: "instituicaoid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_funcionario_statusfuncionario_status",
                        column: x => x.status,
                        principalTable: "statusfuncionario",
                        principalColumn: "statusfuncionarioid",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "animal",
                columns: table => new
                {
                    animalid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    setor = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    especie = table.Column<int>(type: "int", nullable: false),
                    nome = table.Column<string>(type: "longtext", nullable: false),
                    imagemurl = table.Column<string>(type: "longtext", nullable: true),
                    sexo = table.Column<string>(type: "longtext", nullable: false),
                    datanascimento = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    datafalecimento = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    peso = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_animal", x => x.animalid);
                    table.ForeignKey(
                        name: "FK_animal_animalespecie_especie",
                        column: x => x.especie,
                        principalTable: "animalespecie",
                        principalColumn: "animalespecieid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_animal_animalstatus_status",
                        column: x => x.status,
                        principalTable: "animalstatus",
                        principalColumn: "animalstatusid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_animal_setor_setor",
                        column: x => x.setor,
                        principalTable: "setor",
                        principalColumn: "setorid",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "atendimentoveterinario",
                columns: table => new
                {
                    atendimentoveterinarioid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    animal = table.Column<int>(type: "int", nullable: false),
                    funcionariosolicitante = table.Column<int>(type: "int", nullable: false),
                    funcionarioveterinario = table.Column<int>(type: "int", nullable: false),
                    descricao = table.Column<string>(type: "longtext", nullable: false),
                    resultado = table.Column<string>(type: "longtext", nullable: true),
                    observacoes = table.Column<string>(type: "longtext", nullable: true),
                    status = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    data = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_atendimentoveterinario", x => x.atendimentoveterinarioid);
                    table.ForeignKey(
                        name: "FK_atendimentoveterinario_animal_animal",
                        column: x => x.animal,
                        principalTable: "animal",
                        principalColumn: "animalid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_atendimentoveterinario_funcionario_funcionariosolicitante",
                        column: x => x.funcionariosolicitante,
                        principalTable: "funcionario",
                        principalColumn: "funcionarioid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_atendimentoveterinario_funcionario_funcionarioveterinario",
                        column: x => x.funcionarioveterinario,
                        principalTable: "funcionario",
                        principalColumn: "funcionarioid",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "procedimento",
                columns: table => new
                {
                    procedimentoid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    animal = table.Column<int>(type: "int", nullable: false),
                    funcionariotarefa = table.Column<int>(type: "int", nullable: false),
                    descricao = table.Column<string>(type: "longtext", nullable: false),
                    observacoes = table.Column<string>(type: "longtext", nullable: true),
                    dataprocedimento = table.Column<DateOnly>(type: "date", nullable: false),
                    status = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_procedimento", x => x.procedimentoid);
                    table.ForeignKey(
                        name: "FK_procedimento_animal_animal",
                        column: x => x.animal,
                        principalTable: "animal",
                        principalColumn: "animalid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_procedimento_funcionario_funcionariotarefa",
                        column: x => x.funcionariotarefa,
                        principalTable: "funcionario",
                        principalColumn: "funcionarioid",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "transferencia",
                columns: table => new
                {
                    transferenciaid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    animal = table.Column<int>(type: "int", nullable: false),
                    instituicaoorigem = table.Column<int>(type: "int", nullable: false),
                    instituicaodestino = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    datasaida = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    dataentrada = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transferencia", x => x.transferenciaid);
                    table.ForeignKey(
                        name: "FK_transferencia_animal_animal",
                        column: x => x.animal,
                        principalTable: "animal",
                        principalColumn: "animalid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_transferencia_instituicao_instituicaodestino",
                        column: x => x.instituicaodestino,
                        principalTable: "instituicao",
                        principalColumn: "instituicaoid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_transferencia_instituicao_instituicaoorigem",
                        column: x => x.instituicaoorigem,
                        principalTable: "instituicao",
                        principalColumn: "instituicaoid",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_animal_especie",
                table: "animal",
                column: "especie");

            migrationBuilder.CreateIndex(
                name: "IX_animal_setor",
                table: "animal",
                column: "setor");

            migrationBuilder.CreateIndex(
                name: "IX_animal_status",
                table: "animal",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_atendimentoveterinario_animal",
                table: "atendimentoveterinario",
                column: "animal");

            migrationBuilder.CreateIndex(
                name: "IX_atendimentoveterinario_funcionariosolicitante",
                table: "atendimentoveterinario",
                column: "funcionariosolicitante");

            migrationBuilder.CreateIndex(
                name: "IX_atendimentoveterinario_funcionarioveterinario",
                table: "atendimentoveterinario",
                column: "funcionarioveterinario");

            migrationBuilder.CreateIndex(
                name: "IX_funcionario_instituicaoid",
                table: "funcionario",
                column: "instituicaoid");

            migrationBuilder.CreateIndex(
                name: "IX_funcionario_status",
                table: "funcionario",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_procedimento_animal",
                table: "procedimento",
                column: "animal");

            migrationBuilder.CreateIndex(
                name: "IX_procedimento_funcionariotarefa",
                table: "procedimento",
                column: "funcionariotarefa");

            migrationBuilder.CreateIndex(
                name: "IX_setor_instituicaopertence",
                table: "setor",
                column: "instituicaopertence");

            migrationBuilder.CreateIndex(
                name: "IX_transferencia_animal",
                table: "transferencia",
                column: "animal");

            migrationBuilder.CreateIndex(
                name: "IX_transferencia_instituicaodestino",
                table: "transferencia",
                column: "instituicaodestino");

            migrationBuilder.CreateIndex(
                name: "IX_transferencia_instituicaoorigem",
                table: "transferencia",
                column: "instituicaoorigem");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "atendimentoveterinario");

            migrationBuilder.DropTable(
                name: "procedimento");

            migrationBuilder.DropTable(
                name: "transferencia");

            migrationBuilder.DropTable(
                name: "funcionario");

            migrationBuilder.DropTable(
                name: "animal");

            migrationBuilder.DropTable(
                name: "statusfuncionario");

            migrationBuilder.DropTable(
                name: "animalespecie");

            migrationBuilder.DropTable(
                name: "animalstatus");

            migrationBuilder.DropTable(
                name: "setor");

            migrationBuilder.DropTable(
                name: "instituicao");
        }
    }
}
