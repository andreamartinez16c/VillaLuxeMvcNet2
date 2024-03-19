using Microsoft.EntityFrameworkCore;
using VillaLuxeMvcNet.Models;

namespace VillaLuxeMvcNet.Data
{
    public class VillaContext: DbContext
    {
        public VillaContext(DbContextOptions<VillaContext> options) : base(options) { }
        public DbSet<Villa> Villas { get; set; }
        public DbSet<VillaTabla> VillasT { get; set; }
        public DbSet<Provincias> Provincias { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<MisReservas> MisReservas { get; set;}
        public DbSet<Imagen> Imagenes { get; set; }
    }
}
