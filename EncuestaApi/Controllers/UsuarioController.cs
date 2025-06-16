using EncuestaApi.Models.DTOs;
using EncuestaApi.Models.Entities;
using EncuestaApi.Repositories;
using EncuestaApi.Services;
using EncuestaApi.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EncuestaApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        public UsuarioController(Repository<Usuarios> repoUsuarios, UsuarioValidator validador, JwtService service)
        {
            RepoUsuarios = repoUsuarios;
            Validador = validador;
            Service = service;
        }
        public Repository<Usuarios> RepoUsuarios { get; }
        public UsuarioValidator Validador { get; }
        public JwtService Service { get; }

        [Authorize(Roles = "Admin")]
        [HttpPost("registrar")]
        public IActionResult Registrar(RegistrarUsuarioDTO dto)
        {
            if (Validador.ValidateRegister(dto, out List<string> errores))
            {
                Usuarios user = new()
                {
                    Contrasena = dto.Contrasena,
                    NombreUsuario = dto.Nombre,
                    FechaRegistro = DateTime.Now,
                    Rol = dto.EsAdmin
                };
                RepoUsuarios.Insert(user);
                return Ok();
            }
            else
            {
                return BadRequest(errores);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public IActionResult GetUsuarioPorId(int id)
        {
            var usuario = RepoUsuarios.GetId(id);

            if (usuario == null)
            {
                return NotFound(new { mensaje = "Usuario no encontrado." });
            }

            var dto = new UsuarioResumenDTO
            {
                Id = usuario.Id,
                Nombre = usuario.NombreUsuario,
                FechaRegistro = usuario.FechaRegistro,
                EsAdmin = usuario.Rol
            };

            return Ok(dto);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("usuarios")]
        public IActionResult GetUsuarios()
        {
            var usuarios = RepoUsuarios.GetAll();

            var lista = usuarios.Select(u => new UsuarioResumenDTO
            {
                Id = u.Id,
                Nombre = u.NombreUsuario,
                FechaRegistro = u.FechaRegistro,
                EsAdmin = u.Rol
            }).ToList();

            return Ok(lista);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult EliminarUsuario(int id)
        {
            var usuario = RepoUsuarios.GetId(id);
            if (usuario == null)
            {
                return NotFound(new { mensaje = "Usuario no encontrado." });
            }

            var currentId = int.Parse(User.FindFirst("Id")?.Value ?? "0");
            if (id == currentId)
                return BadRequest("No puedes eliminar tu propio usuario.");

            try
            {
                RepoUsuarios.Delete(id);
                return Ok(new { mensaje = "Usuario eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar el usuario.", detalle = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginUsuarioDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre) || string.IsNullOrWhiteSpace(dto.Contrasena))
            {
                return BadRequest("El nombre de usuario y contraseña son obligatorios.");
            }


           
            var usuario = RepoUsuarios.GetAll().FirstOrDefault(u =>
                u.NombreUsuario.Trim().ToLower() == dto.Nombre.Trim().ToLower() &&
                u.Contrasena.Trim() == dto.Contrasena.Trim());

            if (usuario == null)
            {
                return Unauthorized("El usuario o contraseña son incorrectos");
            }

            try
            {
                var token = Service.GenerarToken(usuario);
                if (token == null)
                {
                    return StatusCode(500, "No se pudo generar el token");
                }

                return Ok(new
                {
                    token,
                    esAdmin = usuario.Rol ?? "Usuario",
                    id = usuario.Id,
                    nombre = usuario.NombreUsuario
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error generando token: {ex.Message}");
            }
        }

    }
}
