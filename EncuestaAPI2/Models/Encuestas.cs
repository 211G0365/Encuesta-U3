using System;
using System.Collections.Generic;

namespace EncuestaAPI2.Models;

public partial class Encuestas
{
    public int Id { get; set; }

    public string Titulo { get; set; } = null!;

    public int CreadoPorId { get; set; }

    public bool Activa { get; set; }

    public virtual Usuarios CreadoPor { get; set; } = null!;

    public virtual ICollection<Preguntas> Preguntas { get; set; } = new List<Preguntas>();

    public virtual ICollection<Respuestas> Respuestas { get; set; } = new List<Respuestas>();
}
