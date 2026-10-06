using CatalogoGalactico.Models;

namespace CatalogoGalactico.Data;

public class AlmacenMemoria
{
    private int _ultimoPersonaje;
    private int _ultimaCarta;
    private int _ultimoEvento;

    public List<Personaje> Personajes { get; } =
    [
        new(1, "Luke Skywalker", "Humano", Faccion.Rebelde, "Alianza Rebelde", EstadoPersonaje.Vivo, true, "https://static.wikia.nocookie.net/starwars/images/6/6c/LukeSkywalker-RotJAVA.png/revision/latest?cb=20260214054914"),
        new(2, "Leia Organa", "Humana", Faccion.Rebelde, "Alianza Rebelde", EstadoPersonaje.Vivo, true, "https://static.wikia.nocookie.net/starwars/images/9/9b/Princessleiaheadwithgun.jpg/revision/latest/scale-to-width-down/1000?cb=20240522043127"),
        new(3, "Han Solo", "Humano", Faccion.Rebelde, "Alianza Rebelde", EstadoPersonaje.Vivo, false, "https://static.wikia.nocookie.net/starwars/images/4/49/HanSolo1BBY-TheStarWarsBook.png/revision/latest?cb=20241220234647"),
        new(4, "Darth Vader", "Humano", Faccion.Imperio, "Imperio Galáctico", EstadoPersonaje.Muerto, true, "https://static.wikia.nocookie.net/starwars/images/d/de/DarthVader-TheEmpire2026.png/revision/latest?cb=20260522041242"),
        new(5, "Obi-Wan Kenobi", "Humano", Faccion.Rebelde, "Orden Jedi", EstadoPersonaje.Muerto, true, "https://static.wikia.nocookie.net/starwars/images/c/c5/ObiSWC.png/revision/latest?cb=20250422041447"),
        new(6, "Wilhuff Tarkin", "Humano", Faccion.Imperio, "Imperio Galáctico", EstadoPersonaje.Muerto, false, "https://static.wikia.nocookie.net/starwars/images/2/2a/AdmiralTarkin-BaseSeries3.png/revision/latest?cb=20251111015027"),
        new(7, "Emperador Palpatine", "Humano", Faccion.Imperio, "Imperio Galáctico", EstadoPersonaje.Muerto, true, "https://static.wikia.nocookie.net/starwars/images/e/e2/Palpatine-CEUEEd.png/revision/latest/scale-to-width-down/1000?cb=20250105171652"),
        new(8, "Chewbacca", "Wookiee", Faccion.Rebelde, "Alianza Rebelde", EstadoPersonaje.Vivo, false, "https://static.wikia.nocookie.net/starwars/images/2/25/Chewbacca-SWBC5cvr.png/revision/latest?cb=20260426033709"),
        new(9, "Boba Fett", "Humano", Faccion.Neutral, "Cazarrecompensas", EstadoPersonaje.Vivo, false, "https://static.wikia.nocookie.net/starwars/images/4/46/BobaFett-SWI206.png/revision/latest/scale-to-width-down/1000?cb=20250317160030"),
        //Tarea 2: Personajes agregados y un link de imagen a cada personaje
        new(10, "Cal Kestis", "Humano", Faccion.Neutral, "Ex-Yedi", EstadoPersonaje.Vivo, false, "https://static.wikia.nocookie.net/starwars/images/b/b0/9BBY_Cal.png/revision/latest?cb=20250109160127"),
        new(11, "Trilla Suduri", "Humana", Faccion.Rebelde, "Inquisidora", EstadoPersonaje.Muerto, false, "https://static.wikia.nocookie.net/starwarsjedifallenorder/images/f/fc/Second_Sister.png/revision/latest?cb=20191118211905")

    ];

    public List<CardPersonaje> Cartas { get; } =
    [
        new(1, 1, 85, "Uso de la Fuerza", "Sable de luz verde", 4, "https://static.wikia.nocookie.net/starwars/images/6/6c/LukeSkywalker-RotJAVA.png/revision/latest?cb=20260214054914"),
        new(2, 2, 60, "Liderazgo estratégico", "Blaster", 2, "https://static.wikia.nocookie.net/starwars/images/9/9b/Princessleiaheadwithgun.jpg/revision/latest/scale-to-width-down/1000?cb=20240522043127"),
        new(3, 3, 65, "Piloto del Halcón Milenario", "Blaster DL-44", 3, "https://static.wikia.nocookie.net/starwars/images/4/49/HanSolo1BBY-TheStarWarsBook.png/revision/latest?cb=20241220234647"),
        new(4, 4, 95, "Estrangulamiento con la Fuerza", "Sable de luz rojo", 5, "https://static.wikia.nocookie.net/starwars/images/d/de/DarthVader-TheEmpire2026.png/revision/latest?cb=20260522041242"),
        new(5, 5, 80, "Truco mental Jedi", "Sable de luz azul", 4, "https://static.wikia.nocookie.net/starwars/images/c/c5/ObiSWC.png/revision/latest?cb=20250422041447"),
        new(6, 6, 40, "Doctrina del terror", "Blaster de oficial", 3, "https://static.wikia.nocookie.net/starwars/images/2/2a/AdmiralTarkin-BaseSeries3.png/revision/latest?cb=20251111015027"),
        new(7, 7, 98, "Rayos de la Fuerza", "Sable de luz rojo", 5, "https://static.wikia.nocookie.net/starwars/images/e/e2/Palpatine-CEUEEd.png/revision/latest/scale-to-width-down/1000?cb=20250105171652"),
        new(8, 8, 70, "Fuerza descomunal", "Ballesta Bowcaster", 3, "https://static.wikia.nocookie.net/starwars/images/2/25/Chewbacca-SWBC5cvr.png/revision/latest?cb=20260426033709"),
        new(9, 9, 62, "Rastreo de cazarrecompensas", "Blaster EE-3", 3, "https://static.wikia.nocookie.net/starwars/images/4/46/BobaFett-SWI206.png/revision/latest/scale-to-width-down/1000?cb=20250317160030"),
        new(10, 10, 78, "Manipulación de la Fuerza y combate con sable", "Sable de luz doble", 4, "https://static.wikia.nocookie.net/starwars/images/b/b0/9BBY_Cal.png/revision/latest?cb=20250109160127"),
        new(11, 11, 74, "Persecución implacable", "Sable de luz doble rojo", 4, "https://static.wikia.nocookie.net/starwarsjedifallenorder/images/f/fc/Second_Sister.png/revision/latest?cb=20191118211905")
    ];

    public List<Evento> Eventos { get; } =
    [
        new(1, "Contrato en Mos Eisley", "5 BBY", -5, "Mos Eisley, Tatooine",
            "Un encargo tenso en la cantina más peligrosa del borde exterior.",
            [3, 8, 9], [], null),
        new(2, "Duelo en la Estrella de la Muerte", "Batalla de Yavin", 0, "Estrella de la Muerte",
            "Obi-Wan se enfrenta a Vader mientras el grupo rescata a la princesa.",
            [1, 3, 4, 5, 8], [5], null),
        new(3, "Batalla de Yavin", "Batalla de Yavin", 0, "Yavin 4",
            "La Alianza Rebelde destruye la Estrella de la Muerte.",
            [1, 2, 3, 4, 6, 8], [6], null),
        new(4, "Batalla de Hoth", "3 ABY", 3, "Hoth",
            "El Imperio asalta la base rebelde Echo.",
            [1, 2, 3, 4, 8], [], null),
        new(5, "Batalla de Endor", "4 ABY", 4, "Luna boscosa de Endor",
            "Combate final contra el Emperador y la segunda Estrella de la Muerte.",
            [1, 2, 3, 4, 7, 8], [4, 7], null),
    ];

    public AlmacenMemoria()
    {
        _ultimoPersonaje = Personajes.Max(p => p.Id);
        _ultimaCarta = Cartas.Max(c => c.Id);
        _ultimoEvento = Eventos.Max(e => e.Id);
    }

    public int SiguientePersonajeId() => ++_ultimoPersonaje;
    public int SiguienteCartaId() => ++_ultimaCarta;
    public int SiguienteEventoId() => ++_ultimoEvento;
}
