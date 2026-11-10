using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CatalogoGalactico.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Eventos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Fecha = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Anio = table.Column<int>(type: "int", nullable: false),
                    Ubicacion = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descripcion = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Ganador = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Eventos", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Personajes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Especie = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Faccion = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Afiliacion = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Estado = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FuerzaSensitivo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Image = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personajes", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Cartas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PersonajeId = table.Column<int>(type: "int", nullable: false),
                    Poder = table.Column<int>(type: "int", nullable: false),
                    HabilidadEspecial = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Arma = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NivelPeligrosidad = table.Column<int>(type: "int", nullable: false),
                    ImagenUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cartas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cartas_Personajes_PersonajeId",
                        column: x => x.PersonajeId,
                        principalTable: "Personajes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EventoParticipantes",
                columns: table => new
                {
                    EventoId = table.Column<int>(type: "int", nullable: false),
                    PersonajeId = table.Column<int>(type: "int", nullable: false),
                    Murio = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoParticipantes", x => new { x.EventoId, x.PersonajeId });
                    table.ForeignKey(
                        name: "FK_EventoParticipantes_Eventos_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventoParticipantes_Personajes_PersonajeId",
                        column: x => x.PersonajeId,
                        principalTable: "Personajes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Eventos",
                columns: new[] { "Id", "Anio", "Descripcion", "Fecha", "Ganador", "Nombre", "Ubicacion" },
                values: new object[,]
                {
                    { 1, -5, "Un encargo tenso en la cantina más peligrosa del borde exterior.", "5 BBY", null, "Contrato en Mos Eisley", "Mos Eisley, Tatooine" },
                    { 2, 0, "Obi-Wan se enfrenta a Vader mientras el grupo rescata a la princesa.", "Batalla de Yavin", null, "Duelo en la Estrella de la Muerte", "Estrella de la Muerte" },
                    { 3, 0, "La Alianza Rebelde destruye la Estrella de la Muerte.", "Batalla de Yavin", null, "Batalla de Yavin", "Yavin 4" },
                    { 4, 3, "El Imperio asalta la base rebelde Echo.", "3 ABY", null, "Batalla de Hoth", "Hoth" },
                    { 5, 4, "Combate final contra el Emperador y la segunda Estrella de la Muerte.", "4 ABY", null, "Batalla de Endor", "Luna boscosa de Endor" }
                });

            migrationBuilder.InsertData(
                table: "Personajes",
                columns: new[] { "Id", "Afiliacion", "Especie", "Estado", "Faccion", "FuerzaSensitivo", "Image", "Nombre" },
                values: new object[,]
                {
                    { 1, "Alianza Rebelde", "Humano", "Vivo", "Rebelde", true, "https://static.wikia.nocookie.net/starwars/images/6/6c/LukeSkywalker-RotJAVA.png/revision/latest?cb=20260214054914", "Luke Skywalker" },
                    { 2, "Alianza Rebelde", "Humana", "Vivo", "Rebelde", true, "https://static.wikia.nocookie.net/starwars/images/9/9b/Princessleiaheadwithgun.jpg/revision/latest/scale-to-width-down/1000?cb=20240522043127", "Leia Organa" },
                    { 3, "Alianza Rebelde", "Humano", "Vivo", "Rebelde", false, "https://static.wikia.nocookie.net/starwars/images/4/49/HanSolo1BBY-TheStarWarsBook.png/revision/latest?cb=20241220234647", "Han Solo" },
                    { 4, "Imperio Galáctico", "Humano", "Muerto", "Imperio", true, "https://static.wikia.nocookie.net/starwars/images/d/de/DarthVader-TheEmpire2026.png/revision/latest?cb=20260522041242", "Darth Vader" },
                    { 5, "Orden Jedi", "Humano", "Muerto", "Rebelde", true, "https://static.wikia.nocookie.net/starwars/images/c/c5/ObiSWC.png/revision/latest?cb=20250422041447", "Obi-Wan Kenobi" },
                    { 6, "Imperio Galáctico", "Humano", "Muerto", "Imperio", false, "https://static.wikia.nocookie.net/starwars/images/2/2a/AdmiralTarkin-BaseSeries3.png/revision/latest?cb=20251111015027", "Wilhuff Tarkin" },
                    { 7, "Imperio Galáctico", "Humano", "Muerto", "Imperio", true, "https://static.wikia.nocookie.net/starwars/images/e/e2/Palpatine-CEUEEd.png/revision/latest/scale-to-width-down/1000?cb=20250105171652", "Emperador Palpatine" },
                    { 8, "Alianza Rebelde", "Wookiee", "Vivo", "Rebelde", false, "https://static.wikia.nocookie.net/starwars/images/2/25/Chewbacca-SWBC5cvr.png/revision/latest?cb=20260426033709", "Chewbacca" },
                    { 9, "Cazarrecompensas", "Humano", "Vivo", "Neutral", false, "https://static.wikia.nocookie.net/starwars/images/4/46/BobaFett-SWI206.png/revision/latest/scale-to-width-down/1000?cb=20250317160030", "Boba Fett" },
                    { 10, "Ex-Yedi", "Humano", "Vivo", "Neutral", false, "https://static.wikia.nocookie.net/starwars/images/b/b0/9BBY_Cal.png/revision/latest?cb=20250109160127", "Cal Kestis" },
                    { 11, "Inquisidora", "Humana", "Muerto", "Rebelde", false, "https://static.wikia.nocookie.net/starwarsjedifallenorder/images/f/fc/Second_Sister.png/revision/latest?cb=20191118211905", "Trilla Suduri" }
                });

            migrationBuilder.InsertData(
                table: "Cartas",
                columns: new[] { "Id", "Arma", "HabilidadEspecial", "ImagenUrl", "NivelPeligrosidad", "PersonajeId", "Poder" },
                values: new object[,]
                {
                    { 1, "Sable de luz verde", "Uso de la Fuerza", "https://static.wikia.nocookie.net/starwars/images/6/6c/LukeSkywalker-RotJAVA.png/revision/latest?cb=20260214054914", 4, 1, 85 },
                    { 2, "Blaster", "Liderazgo estratégico", "https://static.wikia.nocookie.net/starwars/images/9/9b/Princessleiaheadwithgun.jpg/revision/latest/scale-to-width-down/1000?cb=20240522043127", 2, 2, 60 },
                    { 3, "Blaster DL-44", "Piloto del Halcón Milenario", "https://static.wikia.nocookie.net/starwars/images/4/49/HanSolo1BBY-TheStarWarsBook.png/revision/latest?cb=20241220234647", 3, 3, 65 },
                    { 4, "Sable de luz rojo", "Estrangulamiento con la Fuerza", "https://static.wikia.nocookie.net/starwars/images/d/de/DarthVader-TheEmpire2026.png/revision/latest?cb=20260522041242", 5, 4, 95 },
                    { 5, "Sable de luz azul", "Truco mental Jedi", "https://static.wikia.nocookie.net/starwars/images/c/c5/ObiSWC.png/revision/latest?cb=20250422041447", 4, 5, 80 },
                    { 6, "Blaster de oficial", "Doctrina del terror", "https://static.wikia.nocookie.net/starwars/images/2/2a/AdmiralTarkin-BaseSeries3.png/revision/latest?cb=20251111015027", 3, 6, 40 },
                    { 7, "Sable de luz rojo", "Rayos de la Fuerza", "https://static.wikia.nocookie.net/starwars/images/e/e2/Palpatine-CEUEEd.png/revision/latest/scale-to-width-down/1000?cb=20250105171652", 5, 7, 98 },
                    { 8, "Ballesta Bowcaster", "Fuerza descomunal", "https://static.wikia.nocookie.net/starwars/images/2/25/Chewbacca-SWBC5cvr.png/revision/latest?cb=20260426033709", 3, 8, 70 },
                    { 9, "Blaster EE-3", "Rastreo de cazarrecompensas", "https://static.wikia.nocookie.net/starwars/images/4/46/BobaFett-SWI206.png/revision/latest/scale-to-width-down/1000?cb=20250317160030", 3, 9, 62 },
                    { 10, "Sable de luz doble", "Manipulación de la Fuerza y combate con sable", "https://static.wikia.nocookie.net/starwars/images/b/b0/9BBY_Cal.png/revision/latest?cb=20250109160127", 4, 10, 78 },
                    { 11, "Sable de luz doble rojo", "Persecución implacable", "https://static.wikia.nocookie.net/starwarsjedifallenorder/images/f/fc/Second_Sister.png/revision/latest?cb=20191118211905", 4, 11, 74 }
                });

            migrationBuilder.InsertData(
                table: "EventoParticipantes",
                columns: new[] { "EventoId", "PersonajeId", "Murio" },
                values: new object[,]
                {
                    { 1, 3, false },
                    { 1, 8, false },
                    { 1, 9, false },
                    { 2, 1, false },
                    { 2, 3, false },
                    { 2, 4, false },
                    { 2, 5, true },
                    { 2, 8, false },
                    { 3, 1, false },
                    { 3, 2, false },
                    { 3, 3, false },
                    { 3, 4, false },
                    { 3, 6, true },
                    { 3, 8, false },
                    { 4, 1, false },
                    { 4, 2, false },
                    { 4, 3, false },
                    { 4, 4, false },
                    { 4, 8, false },
                    { 5, 1, false },
                    { 5, 2, false },
                    { 5, 3, false },
                    { 5, 4, true },
                    { 5, 7, true },
                    { 5, 8, false }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cartas_PersonajeId",
                table: "Cartas",
                column: "PersonajeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventoParticipantes_PersonajeId",
                table: "EventoParticipantes",
                column: "PersonajeId");

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_Anio",
                table: "Eventos",
                column: "Anio");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cartas");

            migrationBuilder.DropTable(
                name: "EventoParticipantes");

            migrationBuilder.DropTable(
                name: "Eventos");

            migrationBuilder.DropTable(
                name: "Personajes");
        }
    }
}
