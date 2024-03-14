using Microsoft.EntityFrameworkCore;
using VillaLuxeMvcNet.Models;

namespace VillaLuxeMvcNet.Data
{
    public class VillaContext: DbContext
    {
        public VillaContext(DbContextOptions<VillaContext> options) : base(options) { }
        public DbSet<Villa> Villas { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
    }
}
