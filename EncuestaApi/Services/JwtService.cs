﻿using EncuestaApi.Models.Entities;
using EncuestaApi.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EncuestaApi.Services
{
    public class JwtService
    {
        public JwtService(IConfiguration configuration, Repository<Usuarios> reposUsuarios)
        {
            Configuration = configuration;
            ReposUsuarios = reposUsuarios;
        }

        public IConfiguration Configuration { get; }
        public Repository<Usuarios> ReposUsuarios { get; }

        public string? GenerarToken(Usuarios usuario)
        {
            if (usuario == null)
            {
                return null;
            }

            // 1. Crear los claims
            List<Claim> claims = new List<Claim>
    {
        new Claim("Id", usuario.Id.ToString()),
        new Claim(ClaimTypes.Name, usuario.NombreUsuario),
        new Claim(ClaimTypes.Role, usuario. Rol?? "Usuario")
    };

            // 2. Crear descriptor del token
            var descriptor = new JwtSecurityToken(
                issuer: Configuration["Jwt:Issuer"],
                audience: Configuration["Jwt:Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
                    SecurityAlgorithms.HmacSha256)
            );

            // 3. Generar y retornar token
            var handler = new JwtSecurityTokenHandler();
            return handler.WriteToken(descriptor);
        }

    }
}
