using EncuestaApi.Models.DTOs;
using EncuestaApi.Models.Entities;
using AutoMapper;

namespace EncuestaApi.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Usuarios, LoginUsuarioDTO>().ReverseMap();


            //Encuestas
            CreateMap<Encuestas, EncuestaDTO>();
            CreateMap<CrearEncuestaDTO, Encuestas>();
            CreateMap<CrearPreguntaDto, Preguntas>();

            //Preguntas
            CreateMap<Preguntas, PreguntaDTO>();
            CreateMap<Preguntas, PreguntasDTO>();


            //Respuestas
            CreateMap<Repuestas, RespuestaDTO>();
            CreateMap<RespuestaDTO, Repuestas>();
            CreateMap<AplicarEncuestaDTO, Repuestas>();
            CreateMap<RespuestaPreguntaDto, Encuestadetalles>();

            //Detallerespuestas
            CreateMap<Encuestadetalles, RespuestaDetalleDTO>();
        }
    }
}
