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

        public async Task<int> GetMaxId()
        {
            if (await this.context.Reservas.CountAsync() == 0) 
                return 1;
            return await this.context.Reservas.MaxAsync(c => c.IdReserva) + 1;
        }

        public async Task CreateReserva(Reserva reserva )
        {
            int id = await GetMaxId();
            reserva.IdReserva = id;
            reserva.IdUsuario = 1;
            Villa villa = await context.Villas.FindAsync(reserva.IdVilla);
            int cantidadDias = (reserva.FechaFin - reserva.FechaInicio).Days;
            decimal precioPorNoche = villa.PrecioNoche;
            decimal precioTotal = precioPorNoche * cantidadDias;
            reserva.PrecioTotal = precioTotal;
            reserva.Estado = "EN PROCESO";
            Reserva res = reserva;

            this.context.Reservas.Add( res );
            this.context.SaveChanges();
        }
    }
}
