using EncuestaApi.Models.Entities;
using EncuestaApi.Models.DTOs;

namespace EncuestaApi.Repositories
{
    public class EstadisticasRepository
    {
        public EncuestaContext Context { get; set; }


        public EstadisticasRepository(EncuestaContext context)
        {
            Context = context;
        }
        public int GetTotalEncuestasCreadas()
        {
            return Context.Encuestas.Count();
        }

        public int GetTotalEncuestasSinResponder()
        {
            return Context.Encuestas
                .Count(e => !Context.Repuestas.Any(r => r.IdEncuesta == e.Id));
        }


        public int GetTotalEncuestasRespondidas()
        {
            return Context.Repuestas.Count();
        }

        public double GetPromedioRespuestasPorEncuesta()
        {
            var totalEncuestas = GetTotalEncuestasCreadas();
            if (totalEncuestas == 0) return 0;

            var totalRespuestas = GetTotalEncuestasRespondidas();
            return Math.Round((double)totalRespuestas / totalEncuestas, 2);
        }
        public int GetTotalAlumnosEntrevistados()
        {
            return Context.Repuestas
                .Select(r => r.AlumnoNumControl)
                .Distinct()
                .Count();
        }
        public List<TotalRespuestasDTO> GetEncuestasConTotalRespuestas()
        {
            var encuestas = Context.Encuestas
                .Select(e => new TotalRespuestasDTO
                {
                    NombreEncuesta = e.Titulo,
                    TotalRespuestas = Context.Repuestas.Count(r => r.IdEncuesta == e.Id)
                })
                .ToList();

            return encuestas;
        }

        //Preguntas
        public int GetTotalPreguntas()
        {
            return Context.Preguntas.Count();
        }
        public List<RespuestasPorPreguntaDTO> GetCantidadRespuestasPorPregunta()
        {
            var resultado = Context.Preguntas.Select(p => new RespuestasPorPreguntaDTO
            {
                IdPregunta = p.Id,
                Descripcion = p.Descripcion,
                CantidadRespuestas = p.Encuestadetalles.Count()
            })
                .ToList();
            return resultado;
        }


  
    }
}
