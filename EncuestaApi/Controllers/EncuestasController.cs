using AutoMapper;
using EncuestaApi.Hubs;
using EncuestaApi.Models.DTOs;
using EncuestaApi.Models.Entities;
using EncuestaApi.Repositories;
using EncuestaApi.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using EncuestaApi.Helpers;

namespace EncuestaApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EncuestasController : ControllerBase
    {
        private readonly Repository<Encuestas> _encuestaRepo;
        private readonly Repository<Preguntas> _preguntaRepo;
        private readonly Repository<Repuestas> _respuestasRepo;
        private readonly EstadisticasRepository _estadisticasRepo;

        private readonly IMapper _mapper;

        public EncuestaValidator Validador { get; }
        private readonly IHubContext<EstadisticasHubs> _hub;

        public EncuestasController(Repository<Encuestas> encuestaRepo,
        Repository<Preguntas> preguntaRepo, Repository<Repuestas> respuestasRepo,
        IMapper mapper, EncuestaValidator validador, EstadisticasRepository estadisticasRepo,
        IHubContext<EstadisticasHubs> hub)
        {
            _encuestaRepo = encuestaRepo;
            _preguntaRepo = preguntaRepo;
            _respuestasRepo = respuestasRepo;
            _estadisticasRepo = estadisticasRepo;
            _hub = hub;
            _mapper = mapper;
            Validador = validador;

        }

        [HttpGet("encuestas")]
        public IActionResult GetAll()
        {
            var encuestas = _encuestaRepo.GetAll();
            var dto = _mapper.Map<IEnumerable<EncuestaDTO>>(encuestas);
            return Ok(dto);
        }


        [HttpGet("{id}")]
        public IActionResult GetxId(int id)
        {
            var encuesta = _encuestaRepo.GetxId(id);
            if (encuesta == null)
            {
                return NotFound();
            }
            var dto = _mapper.Map<EncuestaDTO>(encuesta);
            return Ok(dto);
        }


        [HttpGet("usuario/{idUsuario}")]
        public IActionResult GetByUsuario(int idUsuario)
        {

            var encuestas = _encuestaRepo.GetAll().Where(e => e.IdUsuario == idUsuario).ToList();
            var dto = _mapper.Map<IEnumerable<EncuestaDTO>>(encuestas);
            return Ok(dto);
        }

        [HttpPost("crear")]
        public async Task<IActionResult> CreateEncuesta([FromBody] CrearEncuestaDTO dto)
        {
            if (Validador.Validate(dto, out List<string> errores))
            {
                var idUsuario = int.Parse(User.FindFirst("Id")?.Value ?? "0");

                var encuesta = new Encuestas
                {
                    IdUsuario = idUsuario,
                    Titulo = dto.Titulo,
                    FechaCreacion = DateTime.Now,
                    Preguntas = dto.Preguntas.Select(P => new Preguntas
                    {
                        Descripcion = P.Descripcion,
                        Orden = P.NumeroPregunta
                    }).ToList()
                };
                _encuestaRepo.Insert(encuesta);

                var encuestaDto = new EncuestaDTO
                {
                    Id = encuesta.Id,
                    IdUsuario = encuesta.IdUsuario,
                    Titulo = encuesta.Titulo,
                    FechaCreacion = encuesta.FechaCreacion
                };

                await _hub.Clients.All.SendAsync("ActualizarEstadisticas");

                return Ok(new
                {
                    mensaje = "Encuesta creada correctamente.",
                    encuesta = encuestaDto
                });
            }
            else
            {
                return BadRequest(errores);
            }
        }

        [HttpGet("con-preguntas/{id}")]
        public ActionResult<EncuestaDTO> GetEncuestaConPreguntas(int id)
        {
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"CLAIM -> {claim.Type}: {claim.Value}");
            }

            var idClaim = User.FindFirst("Id");
            if (idClaim == null)
            {
                return Unauthorized("El token no contiene el claim 'Id'");
            }

            var idUsuario = int.Parse(idClaim.Value);

            var encuesta = _encuestaRepo.GetConPreguntas(id);
            if (encuesta == null)
            {
                return NotFound("Encuesta no encontrada");
            }


            return encuesta;
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> EditarEncuesta(int id, [FromBody] EditarEncuestaDTO dto)
        {
            if (!Validador.ValidateEditarEncuesta(dto, out List<string> errores))
                return BadRequest(errores);

            var encuesta = _encuestaRepo.GetId(id);
            if (encuesta == null)
                return NotFound("La encuesta no existe.");

            var idUsuario = int.Parse(User.FindFirst("Id")?.Value ?? "0");
            var rolUsuario = User.FindFirst("esAdmin")?.Value ?? "";

            if (encuesta.IdUsuario != idUsuario && rolUsuario != "Admin")
                return Forbid("No tienes permiso para editar esta encuesta.");

            // si alguien ya la contestó/aplico no se podrá editar
            if (_respuestasRepo.GetAll().Any(r => r.IdEncuesta == id))
                return BadRequest("No se puede editar una encuesta que ya ha sido respondida.");

            encuesta.Titulo = dto.Titulo;
            _encuestaRepo.Update(encuesta);

            foreach (var preguntaDto in dto.Preguntas)
            {
                if (preguntaDto.Id != 0)
                {
                    var pregunta = _preguntaRepo.GetId(preguntaDto.Id);
                    if (pregunta != null)
                    {
                        pregunta.Descripcion = preguntaDto.Descripcion;
                        pregunta.Orden = preguntaDto.NumeroPregunta;
                        _preguntaRepo.Update(pregunta);
                    }
                }
                else
                {
                    var nueva = new Preguntas
                    {
                        IdEncuesta = id,
                        Descripcion = preguntaDto.Descripcion,
                        Orden = preguntaDto.NumeroPregunta
                    };
                    _preguntaRepo.Insert(nueva);
                }
            }

            await _hub.Clients.All.SendAsync("ActualizarEstadisticas");

            return Ok("Encuesta actualizada correctamente.");
        }

        [HttpDelete("preguntas/{id}")]
        public async Task<IActionResult> EliminarPregunta(int id)
        {
            var pregunta = _preguntaRepo.GetId(id);
            if (pregunta == null)
                return NotFound("La pregunta no existe.");

            var encuesta = _encuestaRepo.GetId(pregunta.IdEncuesta);
            if (encuesta == null)
                return NotFound("Encuesta asociada no encontrada.");

            var idUsuario = int.Parse(User.FindFirst("Id")?.Value ?? "0");
            var rolUsuario = User.FindFirst("esAdmin")?.Value ?? "";

            if (encuesta.IdUsuario != idUsuario && rolUsuario != "Admin")
                return Forbid("No tienes permiso para eliminar esta pregunta.");

            if (_respuestasRepo.GetAll().Any(r => r.IdEncuesta == encuesta.Id))
                return BadRequest("No se puede eliminar pregunta de una encuesta ya respondida.");

            _preguntaRepo.Delete(id);
            await _hub.Clients.All.SendAsync("ActualizarEstadisticas");

            return Ok("Pregunta eliminada correctamente.");
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarEncuesta(int id)
        {
            var encuesta = _encuestaRepo.GetId(id);
            if (encuesta == null)
                return NotFound("La encuesta no existe.");

            var idUsuario = int.Parse(User.FindFirst("Id")?.Value ?? "0");
            var rolUsuario = User.FindFirst("esAdmin")?.Value ?? "";


            if (encuesta.IdUsuario != idUsuario && rolUsuario != "Admin")
                return Forbid("No tienes permiso para eliminar esta encuesta.");


            var respuestas = _respuestasRepo.GetAll().Where(r => r.IdEncuesta == id).ToList();
            if (respuestas.Any())
                return BadRequest("No se puede eliminar una encuesta que ya ha sido respondida.");

            _encuestaRepo.Delete(id);
            await _hub.Clients.All.SendAsync("ActualizarEstadisticas");

            return Ok("Encuesta eliminada correctamente.");

        }

        [HttpGet("{id}/alumnos")]
        public IActionResult GetAlumnosQueRespondieronEncuesta(int id)
        {
            var respuestas = _respuestasRepo.GetAll()
                .Where(r => r.IdEncuesta == id)
                .Select(r => new { r.AlumnoNombre, r.Id, r.AlumnoNumControl })
                .Distinct()
                .ToList();

            var alumnos = respuestas.Select(r => new AlumnoRespondioDTO
            {
                IdAlumno = r.Id,
                Nombre = r.AlumnoNombre,
                NumeroControl = r.AlumnoNumControl
            }).ToList();

            return Ok(alumnos);
        }

        [HttpGet("{idEncuesta}/alumno/{idAlumno}/respuestas")]
        public IActionResult GetPreguntasConRespuestasPorAlumno(int idEncuesta, int idAlumno)
        {
            var detallesRespuestas = _encuestaRepo.GetDetallesPorEncuestaYAlumno(idEncuesta, idAlumno);

            if (!detallesRespuestas.Any())
                return NotFound("No se encontraron respuestas para ese alumno y encuesta.");

            var primerDetalle = detallesRespuestas.First();
            var nombreAlumno = primerDetalle.IdRespuestaNavigation.AlumnoNombre;
            var numeroControl = primerDetalle.IdRespuestaNavigation.AlumnoNumControl;

            var dto = new PreguntasConRespuestaPorAlumnoDTO
            {
                IdEncuesta = idEncuesta,
                IdAlumno = idAlumno,
                Nombre = nombreAlumno,
                NumeroControl = numeroControl,
                Preguntas = detallesRespuestas.Select(dr => new PreguntasConRespuestaPorAlumnoDTO.PreguntaConRespuestaDTO
                {
                    IdPregunta = dr.IdPregunta,
                    Descripcion = dr.IdPreguntaNavigation.Descripcion,
                    ValorRespuesta = dr.Valor
                }).ToList()
            };

            return Ok(dto);
        }





        [Authorize(Roles = "Admin")]
        [HttpGet("totalencuestas")]
        public IActionResult GetTotalEncuestas()
        {
            return Ok(_estadisticasRepo.GetTotalEncuestasCreadas());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("totalrespondidas")]
        public async Task<IActionResult> GetTotalEncuestasRespondidas()
        {
            var total = await Task.FromResult(_estadisticasRepo.GetTotalEncuestasRespondidas());
            return Ok(total);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("totalnorespondidas")]
        public async Task<IActionResult> GetTotalEncuestasSinResponder()
        {
            var total = await Task.FromResult(_estadisticasRepo.GetTotalEncuestasSinResponder());
            return Ok(total);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("promedioRespuestasPorEncuesta")]
        public async Task<IActionResult> GetPromedioRespuestasPorEncuesta()
        {
            var total = await Task.FromResult(_estadisticasRepo.GetPromedioRespuestasPorEncuesta());
            return Ok(total);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("EncuestaConTotalRespuestas")]
        public async Task<IActionResult> GetEncuestaConTotalRespuestas()
        {
            var total = await Task.FromResult(_estadisticasRepo.GetEncuestasConTotalRespuestas());
            return Ok(total);
        }

        [HttpGet("totalAlumnosEntrevistados")]
        public async Task<IActionResult> GetTotalAlumnosEntrevistados()
        {
            var total = await Task.FromResult(_estadisticasRepo.GetTotalAlumnosEntrevistados());
            return Ok(total);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("UsuariosAplicandoEncuestas")]
        public IActionResult GetUsuariosAplicandoEncuestas()
        {
            var usuarios = UsuariosActivos.ObtenerUsuariosActivos();
            return Ok(usuarios);
        }
    }
}
