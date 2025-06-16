using System;
using System.Collections.Generic;

namespace EncuestaAPI2.Models;

public partial class Usuarios
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    public virtual ICollection<Encuestas> Encuestas { get; set; } = new List<Encuestas>();
}
