using CatalogoGalactico.Entities;

namespace CatalogoGalactico.Data;

/// <summary>Datos iniciales del catálogo. Se insertan con la migración (HasData).</summary>
public static class SemillaDatos
{
    private static Personaje P(int id, string nombre, string especie, Faccion faccion, string afiliacion,
        EstadoPersonaje estado, bool fuerzaSensitivo, string image) => new()
    {
        Id = id, Nombre = nombre, Especie = especie, Faccion = faccion, Afiliacion = afiliacion,
        Estado = estado, FuerzaSensitivo = fuerzaSensitivo, Image = image
    };

    private static CardPersonaje C(int id, int personajeId, int poder, string habilidad, string arma,
        int peligrosidad, string imagenUrl) => new()
    {
        Id = id, PersonajeId = personajeId, Poder = poder, HabilidadEspecial = habilidad, Arma = arma,
        NivelPeligrosidad = peligrosidad, ImagenUrl = imagenUrl
    };

    private static Evento E(int id, string nombre, string fecha, int anio, string ubicacion, string descripcion) => new()
    {
        Id = id, Nombre = nombre, Fecha = fecha, Anio = anio, Ubicacion = ubicacion, Descripcion = descripcion
    };

    private static EventoParticipante Part(int eventoId, int personajeId, bool murio = false) => new()
    {
        EventoId = eventoId, PersonajeId = personajeId, Murio = murio
    };

    public static readonly Personaje[] Personajes =
    [
        P(1, "Luke Skywalker", "Humano", Faccion.Rebelde, "Alianza Rebelde", EstadoPersonaje.Vivo, true, "https://static.wikia.nocookie.net/starwars/images/6/6c/LukeSkywalker-RotJAVA.png/revision/latest?cb=20260214054914"),
        P(2, "Leia Organa", "Humana", Faccion.Rebelde, "Alianza Rebelde", EstadoPersonaje.Vivo, true, "https://static.wikia.nocookie.net/starwars/images/9/9b/Princessleiaheadwithgun.jpg/revision/latest/scale-to-width-down/1000?cb=20240522043127"),
        P(3, "Han Solo", "Humano", Faccion.Rebelde, "Alianza Rebelde", EstadoPersonaje.Vivo, false, "https://static.wikia.nocookie.net/starwars/images/4/49/HanSolo1BBY-TheStarWarsBook.png/revision/latest?cb=20241220234647"),
        P(4, "Darth Vader", "Humano", Faccion.Imperio, "Imperio Galáctico", EstadoPersonaje.Muerto, true, "https://static.wikia.nocookie.net/starwars/images/d/de/DarthVader-TheEmpire2026.png/revision/latest?cb=20260522041242"),
        P(5, "Obi-Wan Kenobi", "Humano", Faccion.Rebelde, "Orden Jedi", EstadoPersonaje.Muerto, true, "https://static.wikia.nocookie.net/starwars/images/c/c5/ObiSWC.png/revision/latest?cb=20250422041447"),
        P(6, "Wilhuff Tarkin", "Humano", Faccion.Imperio, "Imperio Galáctico", EstadoPersonaje.Muerto, false, "https://static.wikia.nocookie.net/starwars/images/2/2a/AdmiralTarkin-BaseSeries3.png/revision/latest?cb=20251111015027"),
        P(7, "Emperador Palpatine", "Humano", Faccion.Imperio, "Imperio Galáctico", EstadoPersonaje.Muerto, true, "https://static.wikia.nocookie.net/starwars/images/e/e2/Palpatine-CEUEEd.png/revision/latest/scale-to-width-down/1000?cb=20250105171652"),
        P(8, "Chewbacca", "Wookiee", Faccion.Rebelde, "Alianza Rebelde", EstadoPersonaje.Vivo, false, "https://static.wikia.nocookie.net/starwars/images/2/25/Chewbacca-SWBC5cvr.png/revision/latest?cb=20260426033709"),
        P(9, "Boba Fett", "Humano", Faccion.Neutral, "Cazarrecompensas", EstadoPersonaje.Vivo, false, "https://static.wikia.nocookie.net/starwars/images/4/46/BobaFett-SWI206.png/revision/latest/scale-to-width-down/1000?cb=20250317160030"),
        //Tarea 2: Personajes agregados y un link de imagen a cada personaje
        P(10, "Cal Kestis", "Humano", Faccion.Neutral, "Ex-Yedi", EstadoPersonaje.Vivo, false, "https://static.wikia.nocookie.net/starwars/images/b/b0/9BBY_Cal.png/revision/latest?cb=20250109160127"),
        P(11, "Trilla Suduri", "Humana", Faccion.Rebelde, "Inquisidora", EstadoPersonaje.Muerto, false, "https://static.wikia.nocookie.net/starwarsjedifallenorder/images/f/fc/Second_Sister.png/revision/latest?cb=20191118211905"),
    ];

    public static readonly CardPersonaje[] Cartas =
    [
        C(1, 1, 85, "Uso de la Fuerza", "Sable de luz verde", 4, "https://static.wikia.nocookie.net/starwars/images/6/6c/LukeSkywalker-RotJAVA.png/revision/latest?cb=20260214054914"),
        C(2, 2, 60, "Liderazgo estratégico", "Blaster", 2, "https://static.wikia.nocookie.net/starwars/images/9/9b/Princessleiaheadwithgun.jpg/revision/latest/scale-to-width-down/1000?cb=20240522043127"),
        C(3, 3, 65, "Piloto del Halcón Milenario", "Blaster DL-44", 3, "https://static.wikia.nocookie.net/starwars/images/4/49/HanSolo1BBY-TheStarWarsBook.png/revision/latest?cb=20241220234647"),
        C(4, 4, 95, "Estrangulamiento con la Fuerza", "Sable de luz rojo", 5, "https://static.wikia.nocookie.net/starwars/images/d/de/DarthVader-TheEmpire2026.png/revision/latest?cb=20260522041242"),
        C(5, 5, 80, "Truco mental Jedi", "Sable de luz azul", 4, "https://static.wikia.nocookie.net/starwars/images/c/c5/ObiSWC.png/revision/latest?cb=20250422041447"),
        C(6, 6, 40, "Doctrina del terror", "Blaster de oficial", 3, "https://static.wikia.nocookie.net/starwars/images/2/2a/AdmiralTarkin-BaseSeries3.png/revision/latest?cb=20251111015027"),
        C(7, 7, 98, "Rayos de la Fuerza", "Sable de luz rojo", 5, "https://static.wikia.nocookie.net/starwars/images/e/e2/Palpatine-CEUEEd.png/revision/latest/scale-to-width-down/1000?cb=20250105171652"),
        C(8, 8, 70, "Fuerza descomunal", "Ballesta Bowcaster", 3, "https://static.wikia.nocookie.net/starwars/images/2/25/Chewbacca-SWBC5cvr.png/revision/latest?cb=20260426033709"),
        C(9, 9, 62, "Rastreo de cazarrecompensas", "Blaster EE-3", 3, "https://static.wikia.nocookie.net/starwars/images/4/46/BobaFett-SWI206.png/revision/latest/scale-to-width-down/1000?cb=20250317160030"),
        C(10, 10, 78, "Manipulación de la Fuerza y combate con sable", "Sable de luz doble", 4, "https://static.wikia.nocookie.net/starwars/images/b/b0/9BBY_Cal.png/revision/latest?cb=20250109160127"),
        C(11, 11, 74, "Persecución implacable", "Sable de luz doble rojo", 4, "https://static.wikia.nocookie.net/starwarsjedifallenorder/images/f/fc/Second_Sister.png/revision/latest?cb=20191118211905"),
    ];

    public static readonly Evento[] Eventos =
    [
        E(1, "Contrato en Mos Eisley", "5 BBY", -5, "Mos Eisley, Tatooine",
            "Un encargo tenso en la cantina más peligrosa del borde exterior."),
        E(2, "Duelo en la Estrella de la Muerte", "Batalla de Yavin", 0, "Estrella de la Muerte",
            "Obi-Wan se enfrenta a Vader mientras el grupo rescata a la princesa."),
        E(3, "Batalla de Yavin", "Batalla de Yavin", 0, "Yavin 4",
            "La Alianza Rebelde destruye la Estrella de la Muerte."),
        E(4, "Batalla de Hoth", "3 ABY", 3, "Hoth",
            "El Imperio asalta la base rebelde Echo."),
        E(5, "Batalla de Endor", "4 ABY", 4, "Luna boscosa de Endor",
            "Combate final contra el Emperador y la segunda Estrella de la Muerte."),
    ];

    public static readonly EventoParticipante[] Participaciones =
    [
        // 1. Contrato en Mos Eisley
        Part(1, 3), Part(1, 8), Part(1, 9),
        // 2. Duelo en la Estrella de la Muerte (muere Obi-Wan)
        Part(2, 1), Part(2, 3), Part(2, 4), Part(2, 5, murio: true), Part(2, 8),
        // 3. Batalla de Yavin (muere Tarkin)
        Part(3, 1), Part(3, 2), Part(3, 3), Part(3, 4), Part(3, 6, murio: true), Part(3, 8),
        // 4. Batalla de Hoth
        Part(4, 1), Part(4, 2), Part(4, 3), Part(4, 4), Part(4, 8),
        // 5. Batalla de Endor (mueren Vader y Palpatine)
        Part(5, 1), Part(5, 2), Part(5, 3), Part(5, 4, murio: true), Part(5, 7, murio: true), Part(5, 8),
    ];
}
