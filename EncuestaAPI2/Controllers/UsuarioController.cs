using EncuestaAPI2.DTOs;
using EncuestaAPI2.Models;
using EncuestaAPI2.Repositories;
using EncuestaAPI2.Services;
using EncuestaAPI2.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EncuestaAPI2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        public UsuarioController(Repository<Usuarios> repository, UsuarioValidator validador, JwtService service)
        {
            Repository = repository;
            Validador = validador;
            Service = service;
        }

        public Repository<Usuarios> Repository { get; }
        public UsuarioValidator Validador { get; }
        public JwtService Service { get; }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Registrar(UsuarioDTO dto)
        {
            if (Validador.Validate(dto, out List<string> errores))
            {
                Usuarios user = new()
                {
                    Password = dto.Contrasena,
                    Username = dto.Nombre,
                    Role = dto.Rol,
               
                };
                Repository.Insert(user);
                return Ok();
            }
            else
            {
                return BadRequest(errores);
            }
        }

        //endpoint para eliminar usuario
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult EliminarUsuario(int id)
        {
            var usuario = Repository.Get(id);
            if (usuario == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            Repository.Delete(id);
            return Ok("Usuario eliminado correctamente.");
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(LoginDTO dto)
        {
            var token = Service.GenerarToken(dto);

            if (token == null)
            {
                return Unauthorized("El usuario o contraseña son incorrectos.");
            }
            else
            {
                return Ok(token);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("todos")]
        public IActionResult ObtenerUsuarios()
        {
            var usuarios = Repository.GetAll();
            return Ok(usuarios);
        }

    }
}
