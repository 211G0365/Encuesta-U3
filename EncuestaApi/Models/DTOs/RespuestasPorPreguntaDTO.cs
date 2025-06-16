namespace EncuestaApi.Models.DTOs
{
    public class RespuestasPorPreguntaDTO
    {
        public int IdPregunta { get; set; }
        public string Descripcion { get; set; } = null!;
        public int CantidadRespuestas { get; set; }
    }
}
