using System;
using System.Collections.Generic;

namespace EncuestaApi.Models.Entities;

public partial class Repuestas
{
    public int Id { get; set; }

    public int IdEncuesta { get; set; }

    public int IdUsuario { get; set; }

    public string AlumnoNombre { get; set; } = null!;

    public string AlumnoNumControl { get; set; } = null!;

    public DateTime Fecha { get; set; }

    public virtual ICollection<Encuestadetalles> Encuestadetalles { get; set; } = new List<Encuestadetalles>();

    public virtual Encuestas IdEncuestaNavigation { get; set; } = null!;

    public virtual Usuarios IdUsuarioNavigation { get; set; } = null!;
}
