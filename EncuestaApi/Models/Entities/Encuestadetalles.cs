using System;
using System.Collections.Generic;

namespace EncuestaApi.Models.Entities;

public partial class Encuestadetalles
{
    public int Id { get; set; }

    public int IdRespuesta { get; set; }

    public int IdPregunta { get; set; }

    public int Valor { get; set; }

    public virtual Preguntas IdPreguntaNavigation { get; set; } = null!;

    public virtual Repuestas IdRespuestaNavigation { get; set; } = null!;
}
