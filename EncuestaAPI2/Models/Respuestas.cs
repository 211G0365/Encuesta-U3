using System;
using System.Collections.Generic;

namespace EncuestaAPI2.Models;

public partial class Respuestas
{
    public int Id { get; set; }

    public int EncuestaId { get; set; }

    public int PreguntaId { get; set; }

    public string NumeroDeControl { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public int Valor { get; set; }

    public virtual Encuestas Encuesta { get; set; } = null!;

    public virtual Preguntas Pregunta { get; set; } = null!;
}
