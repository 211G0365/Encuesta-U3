using EncuestaApi.Helpers;
using Microsoft.AspNetCore.SignalR;

namespace EncuestaApi.Hubs
{
    public class EstadisticasHubs:Hub
    {
        public Task RegistrarAlumno(string nombre)
        {
            if (!string.IsNullOrEmpty(nombre))
            {
                UsuariosActivos.AgregarUsuario(Context.ConnectionId, nombre);
            }

            // Notificar usuarios activos y estadísticas
            return Task.WhenAll(
                Clients.All.SendAsync("UsuariosActivos", UsuariosActivos.ObtenerUsuariosActivos()),
                Clients.All.SendAsync("ActualizarEstadisticas")
            );
        }




        public override async Task OnDisconnectedAsync(Exception exception)
        {
            UsuariosActivos.RemoverUsuario(Context.ConnectionId);

            await Task.WhenAll(
                Clients.All.SendAsync("UsuariosActivos", UsuariosActivos.ObtenerUsuariosActivos()),
                Clients.All.SendAsync("ActualizarEstadisticas")
            );

            await base.OnDisconnectedAsync(exception);
        }

        public Task<List<string>> GetUsuariosActivos()
        {
            return Task.FromResult(UsuariosActivos.ObtenerUsuariosActivos());
        }

    }
}
