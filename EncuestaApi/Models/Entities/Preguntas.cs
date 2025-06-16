using System;
using System.Collections.Generic;

namespace EncuestaApi.Models.Entities;

public partial class Preguntas
{
    public int Id { get; set; }

    public int IdEncuesta { get; set; }

    public string Descripcion { get; set; } = null!;

    public int Orden { get; set; }

    public virtual ICollection<Encuestadetalles> Encuestadetalles { get; set; } = new List<Encuestadetalles>();

    public virtual Encuestas IdEncuestaNavigation { get; set; } = null!;
}
