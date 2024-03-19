using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using VillaLuxeMvcNet.Data;
using VillaLuxeMvcNet.Models;

#region
/*
alter VIEW v_villas
AS
SELECT 
    v.idvilla,
    v.nombre AS nombre_villa,
    v.direccion,
    v.descripcion,
    v.comodidades,
    v.personas,
    v.numhabitaciones,
    v.numbanios,
    v.ubicacion,
    v.precionoche,
    u.nombre AS nombre_usuario,
    p.nombre AS nombre_provincia,
    i.imagen
FROM 
    Villas v
INNER JOIN 
    Usuarios u ON v.idusuario = u.idusuario
INNER JOIN 
    Provincias p ON v.idprovincia = p.idprovincia
LEFT JOIN 
    Imagenes i ON v.idvilla = i.idvilla
;
go
create procedure SP_VILLAS_UNICAS_PRIMERA_IMAGEN
as
	select distinct nombre_villa,
    idvilla, direccion, descripcion, comodidades,
    personas, numhabitaciones, numbanios, ubicacion,
    precionoche, nombre_usuario, nombre_provincia, imagen
	from v_villas 
	where Imagen like '%_1.%';
go*/
#endregion

namespace VillaLuxeMvcNet.Repositories
{
    public class RepositoryVillas
    {
        private VillaContext context;
        public RepositoryVillas(VillaContext context)
        {
            this.context = context;
        }

        /*public async Task<List<Villa>> GetVillasAsync()
        {
            string sql = "SP_ALL_VILLAS";
            var consulta = this.context.Villas.FromSqlRaw(sql);
            return await consulta.ToListAsync();
        }*/

        public async Task<VillaTabla> FindVillaTAsync(int idvilla)
        {
            var consulta = from datos in this.context.VillasT
                           where datos.IdVilla == idvilla
                           select datos;
            return await consulta.FirstOrDefaultAsync();
        }
        
        public async Task<Villa> FindVillaAsync(int idvilla)
        {
            var consulta = from datos in this.context.Villas
                           where datos.IdVilla == idvilla
                           select datos;
            return await consulta.FirstOrDefaultAsync();
        }

        public async Task<List<Villa>> GetVillasUnicasAsync()
        {
            string sql = "SP_VILLAS_UNICAS_PRIMERA_IMAGEN";
            var consulta = this.context.Villas.FromSqlRaw(sql);
            return await consulta.ToListAsync();
        }

        public async Task<int> GetMaxIdReserva()
        {
            if (await this.context.Reservas.CountAsync() == 0)
                return 1;
            return await this.context.Reservas.MaxAsync(c => c.IdReserva) + 1;
        }


        public async Task CreateReserva(Reserva reserva, int idusuario)
        {
            if (reserva.FechaFin <= reserva.FechaInicio)
            {
                throw new Exception("La fecha de fin debe ser posterior a la fecha de inicio.");
            }
            // Verificar disponibilidad de fechas
            bool fechasDisponibles = await CheckFechasDisponibles(reserva.IdVilla, reserva.FechaInicio, reserva.FechaFin);

            if (fechasDisponibles)
            {
                int id = await GetMaxIdReserva();
                reserva.IdReserva = id;
                reserva.IdUsuario = idusuario; // Aquí deberías establecer el ID del usuario real
                Villa villa = await context.Villas.FindAsync(reserva.IdVilla);
                int cantidadDias = (reserva.FechaFin - reserva.FechaInicio).Days;
                decimal precioPorNoche = villa.PrecioNoche;
                decimal precioTotal = precioPorNoche * cantidadDias;
                reserva.PrecioTotal = precioTotal;
                reserva.Estado = "EN PROCESO";

                this.context.Reservas.Add(reserva);
                this.context.SaveChanges();
            }
            else
            {
                // Lógica para manejar fechas no disponibles, por ejemplo, lanzar una excepción o devolver un mensaje de error
                throw new Exception("Las fechas seleccionadas no están disponibles.");
            }
        }
        private async Task<bool> CheckFechasDisponibles(int idVilla, DateTime fechaInicio, DateTime fechaFin)
        {
            // Verificar si hay reservas existentes que se superponen con las fechas seleccionadas
            var reservasSuperpuestas = await context.Reservas
                .Where(r => r.IdVilla == idVilla &&
                    (
                        (fechaInicio >= r.FechaInicio && fechaInicio < r.FechaFin) || // Nueva reserva comienza dentro de una reserva existente
                        (fechaFin > r.FechaInicio && fechaFin <= r.FechaFin) || // Nueva reserva termina dentro de una reserva existente
                        (fechaInicio <= r.FechaInicio && fechaFin >= r.FechaFin) // Nueva reserva incluye completamente una reserva existente
                    )
                )
                .ToListAsync();

            // Si no hay reservas superpuestas, las fechas están disponibles
            return reservasSuperpuestas.Count == 0;
        }

        public async Task<List<Reserva>> GetReservasByIdVillaAsync(int idVilla)
        {
            return await this.context.Reservas.Where(x => x.IdVilla == idVilla).ToListAsync();
        }
        public async Task <List<DateTime>> GetFechasReservadasByIdVillaAsync(int idVilla)
        {
            List<DateTime> list = new List<DateTime>();
            List<Reserva> listaReservas = await this.GetReservasByIdVillaAsync(idVilla);
            foreach (var item in listaReservas)
            {
                int totalDias= item.FechaFin.Subtract(item.FechaInicio).Days;
                list.Add(item.FechaInicio);
                for(int i = 1; i <= totalDias; i++)
                {
                    list.Add(item.FechaInicio.AddDays(i));
                }
            }
            
            return list;
        }

        public async Task<List<MisReservas>> GetMisReservas(int idusuario)
        {
            List<MisReservas> list = await context.MisReservas
                .Where(r => r.IdUsuario == idusuario)
                .ToListAsync();
            if (list.Count == 0)
            {
                return null;
            }
            else
            {
                return list;
            }
        }

        public async Task<Reserva> FindReserva(int idReserva)
        {
            return await this.context.Reservas.FirstOrDefaultAsync(
                x => x.IdReserva == idReserva);
        }

        public async Task DeleteReserva(int idReserva)
        {
            Reserva reserva = await this.FindReserva(idReserva);
            if (reserva != null)
            {
                this.context.Reservas.Remove(reserva);
                await this.context.SaveChangesAsync();
            }
        }

        public async Task DeleteVilla(int idVilla)
        {
            VillaTabla villa = await this.FindVillaTAsync(idVilla);
            if(villa != null)
            {
                this.context.VillasT.Remove(villa);
                await this.context.SaveChangesAsync();
            }
        }

        public async Task<List<MisReservas>> GetReservasByIdVillaVistaAsync(int idVilla)
        {
            return await this.context.MisReservas.Where(x => x.IdVilla == idVilla).ToListAsync();
        }

        public async Task<int> GetMaxVilla()
        {
            if (await this.context.VillasT.CountAsync() == 0)
                return 1;
            return await this.context.VillasT.MaxAsync(c => c.IdVilla) + 1;
        }
        public async Task<VillaTabla> CreateVillaAsync(VillaTabla villa)
        {
            villa.IdVilla = await GetMaxVilla();
            context.VillasT.Add(villa);
            await context.SaveChangesAsync();
            return villa;
        }

        public async Task EditVilla(VillaTabla villa)
        {
            // Buscar la villa existente en la base de datos
            var villaExistente = await context.VillasT.FindAsync(villa.IdVilla);

                // Actualizar los campos de la villa con los valores proporcionados
                villaExistente.Nombre = villa.Nombre;
                villaExistente.Direccion = villa.Direccion;
                villaExistente.Descripcion = villa.Descripcion;
                villaExistente.Comodidades = villa.Comodidades;
                villaExistente.Personas = villa.Personas;
                villaExistente.NumHabitaciones = villa.NumHabitaciones;
                villaExistente.NumBanios = villa.NumBanios;
                villaExistente.Ubicacion = villa.Ubicacion;
                villaExistente.PrecioNoche = villa.PrecioNoche;
                villaExistente.IdProvincia = villa.IdProvincia;
                villaExistente.ImagenCollage = villa.ImagenCollage;

                // Guardar los cambios en la base de datos
                await context.SaveChangesAsync();
        }

        public async Task<List<Provincias>> GetProvincias()
        {
            return await context.Provincias.ToListAsync();
        }

        public async Task<List<Imagen>> GetImagenesVilla(int idvilla)
        {
            var consulta = from datos in this.context.Imagenes
                           where datos.IdVilla == idvilla
                           select datos;
            return await consulta.ToListAsync();
        }

        public async Task<Imagen> FindImagenVilla(int idimagen)
        {
            var consulta = from datos in this.context.Imagenes
                           where datos.IdImagen == idimagen
                           select datos;
            return await consulta.FirstOrDefaultAsync();
        }

        public async Task DeleteImagenes(int idimagen)
        {
            Imagen imagen = await this.FindImagenVilla(idimagen);
            if (imagen != null)
            {
                this.context.Imagenes.Remove(imagen);
                await this.context.SaveChangesAsync();
            }
        }

        public async Task InsertarImagenes(int idVilla,string url)
        {
            Imagen imagen = new Imagen();
            imagen.IdImagen = 0;
            imagen.IdVilla = idVilla;
            imagen.Imgn = url;
            await context.Imagenes.AddAsync(imagen);
            await context.SaveChangesAsync();
        }

        public async Task DeleteImgVilla(int idimagen)
        {
            Imagen imagen = await this.FindImagenVilla(idimagen);
            if (imagen != null)
            {
                this.context.Imagenes.Remove(imagen);
                await this.context.SaveChangesAsync();
            }

        }
    }

}
