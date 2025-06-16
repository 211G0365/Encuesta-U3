using System;
using System.Collections.Generic;

namespace EncuestaApi.Models.Entities;

public partial class Usuarios
{
    public int Id { get; set; }

    public string NombreUsuario { get; set; } = null!;

    public string Contrasena { get; set; } = null!;

    public DateTime FechaRegistro { get; set; }

    public string Rol { get; set; } = null!;

    public virtual ICollection<Encuestas> Encuestas { get; set; } = new List<Encuestas>();

    public virtual ICollection<Repuestas> Repuestas { get; set; } = new List<Repuestas>();
}
