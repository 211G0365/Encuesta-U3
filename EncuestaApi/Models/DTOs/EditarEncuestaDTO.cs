﻿namespace EncuestaApi.Models.DTOs
{
    public class EditarEncuestaDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = null!;

        public List<EditarPreguntaDTO> Preguntas { get; set; }
    }
}
