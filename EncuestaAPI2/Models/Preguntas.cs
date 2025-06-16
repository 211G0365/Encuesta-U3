using System;
using System.Collections.Generic;

namespace EncuestaAPI2.Models;

public partial class Preguntas
{
    public int Id { get; set; }

    public int EncuestaId { get; set; }

    public string Texto { get; set; } = null!;

    public int Orden { get; set; }

    public virtual Encuestas Encuesta { get; set; } = null!;

    public virtual ICollection<Respuestas> Respuestas { get; set; } = new List<Respuestas>();
}
