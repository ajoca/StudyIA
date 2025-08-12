using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace study_ia_web_app.Migrations
{
    /// <inheritdoc />
    public partial class Mig1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Archivos",
                columns: table => new
                {
                    ArchivoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NombreArchivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RutaArchivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TextoExtraido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaSubida = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Archivos", x => x.ArchivoId);
                });

            migrationBuilder.CreateTable(
                name: "PlanEstudios",
                columns: table => new
                {
                    PlanEstudioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArchivoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanEstudios", x => x.PlanEstudioId);
                    table.ForeignKey(
                        name: "FK_PlanEstudios_Archivos_ArchivoId",
                        column: x => x.ArchivoId,
                        principalTable: "Archivos",
                        principalColumn: "ArchivoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Quizzes",
                columns: table => new
                {
                    QuizId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArchivoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isQuiz = table.Column<bool>(type: "bit", nullable: false),
                    FechaGenerado = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quizzes", x => x.QuizId);
                    table.ForeignKey(
                        name: "FK_Quizzes_Archivos_ArchivoId",
                        column: x => x.ArchivoId,
                        principalTable: "Archivos",
                        principalColumn: "ArchivoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Resumens",
                columns: table => new
                {
                    ResumenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArchivoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Introduccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Conclusion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isResumen = table.Column<bool>(type: "bit", nullable: false),
                    FechaGenerado = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resumens", x => x.ResumenId);
                    table.ForeignKey(
                        name: "FK_Resumens_Archivos_ArchivoId",
                        column: x => x.ArchivoId,
                        principalTable: "Archivos",
                        principalColumn: "ArchivoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TareasEstudios",
                columns: table => new
                {
                    TareaEstudioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlanEstudioId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TareasEstudios", x => x.TareaEstudioId);
                    table.ForeignKey(
                        name: "FK_TareasEstudios_PlanEstudios_PlanEstudioId",
                        column: x => x.PlanEstudioId,
                        principalTable: "PlanEstudios",
                        principalColumn: "PlanEstudioId");
                });

            migrationBuilder.CreateTable(
                name: "Preguntas",
                columns: table => new
                {
                    PreguntaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuizId = table.Column<int>(type: "int", nullable: false),
                    Enunciado = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Preguntas", x => x.PreguntaId);
                    table.ForeignKey(
                        name: "FK_Preguntas_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "QuizId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Puntos",
                columns: table => new
                {
                    PuntoClaveId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResumenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Texto = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Puntos", x => x.PuntoClaveId);
                    table.ForeignKey(
                        name: "FK_Puntos_Resumens_ResumenId",
                        column: x => x.ResumenId,
                        principalTable: "Resumens",
                        principalColumn: "ResumenId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Alternativas",
                columns: table => new
                {
                    AlternativaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PreguntaId = table.Column<int>(type: "int", nullable: false),
                    Texto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EsCorrecta = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alternativas", x => x.AlternativaId);
                    table.ForeignKey(
                        name: "FK_Alternativas_Preguntas_PreguntaId",
                        column: x => x.PreguntaId,
                        principalTable: "Preguntas",
                        principalColumn: "PreguntaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alternativas_PreguntaId",
                table: "Alternativas",
                column: "PreguntaId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanEstudios_ArchivoId",
                table: "PlanEstudios",
                column: "ArchivoId");

            migrationBuilder.CreateIndex(
                name: "IX_Preguntas_QuizId",
                table: "Preguntas",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_Puntos_ResumenId",
                table: "Puntos",
                column: "ResumenId");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_ArchivoId",
                table: "Quizzes",
                column: "ArchivoId");

            migrationBuilder.CreateIndex(
                name: "IX_Resumens_ArchivoId",
                table: "Resumens",
                column: "ArchivoId");

            migrationBuilder.CreateIndex(
                name: "IX_TareasEstudios_PlanEstudioId",
                table: "TareasEstudios",
                column: "PlanEstudioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alternativas");

            migrationBuilder.DropTable(
                name: "Puntos");

            migrationBuilder.DropTable(
                name: "TareasEstudios");

            migrationBuilder.DropTable(
                name: "Preguntas");

            migrationBuilder.DropTable(
                name: "Resumens");

            migrationBuilder.DropTable(
                name: "PlanEstudios");

            migrationBuilder.DropTable(
                name: "Quizzes");

            migrationBuilder.DropTable(
                name: "Archivos");
        }
    }
}
