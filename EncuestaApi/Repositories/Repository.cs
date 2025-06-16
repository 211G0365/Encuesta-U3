using EncuestaApi.Models;
using EncuestaApi.Models.DTOs;
using EncuestaApi.Models.Entities;
using Microsoft.EntityFrameworkCore;


namespace EncuestaApi.Repositories
{
    public class Repository<T> where T : class
    {
        public EncuestaContext Context { get; set; }

        public Repository(EncuestaContext context)
        {
            Context = context;
        }

        public IEnumerable<T> GetAll()
        {
            return Context.Set<T>();
        }

        public T? GetId(object id)
        {
            return Context.Find<T>(id);
        }

        public Encuestas? GetxId(int id)
        {
            return Context.Encuestas
               .Include(e => e.Preguntas.OrderBy(x => x.Orden))
               .FirstOrDefault(e => e.Id == id);
        }
        public EncuestaDTO? GetConPreguntas(int id)
        {
            return Context.Encuestas
                .Where(e => e.Id == id)
                .Select(e => new EncuestaDTO
                {
                    Id = e.Id,
                    Titulo = e.Titulo,
                    Preguntas = e.Preguntas
                        .OrderBy(p => p.Orden)
                        .Select(p => new PreguntasDTO
                        {
                            Id = p.Id,
                            Descripcion = p.Descripcion,
                            NumeroPregunta = p.Orden
                        }).ToList()
                }).FirstOrDefault();


        }
        public List<Encuestadetalles> GetDetallesPorEncuestaYAlumno(int idEncuesta, int idAlumno)
        {
            // Primero obtenemos las respuestas del alumno para la encuesta
            var respuestasAlumno = Context.Repuestas
                .Where(r => r.IdEncuesta == idEncuesta && r.Id == idAlumno)
                .Select(r => r.Id)
                .ToList();

            if (!respuestasAlumno.Any())
                return new List<Encuestadetalles>();

            // Luego traemos los detalles incluyendo las preguntas
            var detalles = Context.Encuestadetalles
                .Include(dr => dr.IdPreguntaNavigation)
                .Include(d => d.IdRespuestaNavigation)
                .Where(dr => respuestasAlumno.Contains(dr.IdRespuesta))
                .ToList();

            return detalles;
        }



        //CRUD	
        public void Insert(T entity)
        {
            Context.Set<T>().Add(entity);
            Context.SaveChanges();
        }

        public void Update(T entity)
        {
            Context.Set<T>().Update(entity);
            Context.SaveChanges();
        }
        public void Delete(object id)
        {
            var entity = Context.Find<T>(id);
            if (entity != null)
            {
                Context.Remove(entity);
                Context.SaveChanges();
            }

        }


    }
}
