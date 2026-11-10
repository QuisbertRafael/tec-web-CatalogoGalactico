using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogoGalactico.Migrations
{
    /// <inheritdoc />
    public partial class AgregarExperiencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "XpMotivo",
                table: "EventoParticipantes",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "XpOtorgada",
                table: "EventoParticipantes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "EventoParticipantes",
                keyColumns: new[] { "EventoId", "PersonajeId" },
                keyValues: new object[] { 1, 3 },
                columns: new[] { "XpMotivo", "XpOtorgada" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "EventoParticipantes",
                keyColumns: new[] { "EventoId", "PersonajeId" },
                keyValues: new object[] { 1, 8 },
                columns: new[] { "XpMotivo", "XpOtorgada" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "EventoParticipantes",
                keyColumns: new[] { "EventoId", "PersonajeId" },
                keyValues: new object[] { 1, 9 },
                columns: new[] { "XpMotivo", "XpOtorgada" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "EventoParticipantes",
                keyColumns: new[] { "EventoId", "PersonajeId" },
                keyValues: new object[] { 2, 1 },
                columns: new[] { "XpMotivo", "XpOtorgada" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "EventoParticipantes",
                keyColumns: new[] { "EventoId", "PersonajeId" },
                keyValues: new object[] { 2, 3 },
                columns: new[] { "XpMotivo", "XpOtorgada" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "EventoParticipantes",
                keyColumns: new[] { "EventoId", "PersonajeId" },
                keyValues: new object[] { 2, 4 },
                columns: new[] { "XpMotivo", "XpOtorgada" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "EventoParticipantes",
                keyColumns: new[] { "EventoId", "PersonajeId" },
                keyValues: new object[] { 2, 5 },
                columns: new[] { "XpMotivo", "XpOtorgada" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "EventoParticipantes",
                keyColumns: new[] { "EventoId", "PersonajeId" },
                keyValues: new object[] { 2, 8 },
                columns: new[] { "XpMotivo", "XpOtorgada" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "EventoParticipantes",
                keyColumns: new[] { "EventoId", "PersonajeId" },
                keyValues: new object[] { 3, 1 },
                columns: new[] { "XpMotivo", "XpOtorgada" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "EventoParticipantes",
                keyColumns: new[] { "EventoId", "PersonajeId" },
                keyValues: new object[] { 3, 2 },
                columns: new[] { "XpMotivo", "XpOtorgada" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "EventoParticipantes",
                keyColumns: new[] { "EventoId", "PersonajeId" },
                keyValues: new object[] { 3, 3 },
                columns: new[] { "XpMotivo", "XpOtorgada" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "EventoParticipantes",
                keyColumns: new[] { "EventoId", "PersonajeId" },
                keyValues: new object[] { 3, 4 },
                columns: new[] { "XpMotivo", "XpOtorgada" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "EventoParticipantes",
                keyColumns: new[] { "EventoId", "PersonajeId" },
                keyValues: new object[] { 3, 6 },
                columns: new[] { "XpMotivo", "XpOtorgada" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "EventoParticipantes",
                keyColumns: new[] { "EventoId", "PersonajeId" },
                keyValues: new object[] { 3, 8 },
                columns: new[] { "XpMotivo", "XpOtorgada" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "EventoParticipantes",
                keyColumns: new[] { "EventoId", "PersonajeId" },
                keyValues: new object[] { 4, 1 },
                columns: new[] { "XpMotivo", "XpOtorgada" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "EventoParticipantes",
                keyColumns: new[] { "EventoId", "PersonajeId" },
                keyValues: new object[] { 4, 2 },
                columns: new[] { "XpMotivo", "XpOtorgada" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "EventoParticipantes",
                keyColumns: new[] { "EventoId", "PersonajeId" },
                keyValues: new object[] { 4, 3 },
                columns: new[] { "XpMotivo", "XpOtorgada" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "EventoParticipantes",
                keyColumns: new[] { "EventoId", "PersonajeId" },
                keyValues: new object[] { 4, 4 },
                columns: new[] { "XpMotivo", "XpOtorgada" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "EventoParticipantes",
                keyColumns: new[] { "EventoId", "PersonajeId" },
                keyValues: new object[] { 4, 8 },
                columns: new[] { "XpMotivo", "XpOtorgada" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "EventoParticipantes",
                keyColumns: new[] { "EventoId", "PersonajeId" },
                keyValues: new object[] { 5, 1 },
                columns: new[] { "XpMotivo", "XpOtorgada" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "EventoParticipantes",
                keyColumns: new[] { "EventoId", "PersonajeId" },
                keyValues: new object[] { 5, 2 },
                columns: new[] { "XpMotivo", "XpOtorgada" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "EventoParticipantes",
                keyColumns: new[] { "EventoId", "PersonajeId" },
                keyValues: new object[] { 5, 3 },
                columns: new[] { "XpMotivo", "XpOtorgada" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "EventoParticipantes",
                keyColumns: new[] { "EventoId", "PersonajeId" },
                keyValues: new object[] { 5, 4 },
                columns: new[] { "XpMotivo", "XpOtorgada" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "EventoParticipantes",
                keyColumns: new[] { "EventoId", "PersonajeId" },
                keyValues: new object[] { 5, 7 },
                columns: new[] { "XpMotivo", "XpOtorgada" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "EventoParticipantes",
                keyColumns: new[] { "EventoId", "PersonajeId" },
                keyValues: new object[] { 5, 8 },
                columns: new[] { "XpMotivo", "XpOtorgada" },
                values: new object[] { null, 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "XpMotivo",
                table: "EventoParticipantes");

            migrationBuilder.DropColumn(
                name: "XpOtorgada",
                table: "EventoParticipantes");
        }
    }
}
