using VillaLuxeMvcNet.Models;

namespace VillaLuxeMvcNet.Repositories
{
    public interface IRepositoryVillas
    {
        Task<VillaTabla> CreateVillaAsync(VillaTabla villa);
        Task CreateReserva(Reserva reserva, int idusuario);
        Task DeleteImagenes(int idimagen);
        Task DeleteReserva(int idReserva);
        Task DeleteVilla(int idVilla);
        Task<Reserva> FindReserva(int idReserva);
        Task<VillaTabla> FindVillaTAsync(int idvilla);
        Task<Villa> FindVillaAsync(int idvilla);
        Task<VillaFechasResevadas> FindVillaFechaReservadasAsync(int idvilla);
        Task<Imagen> FindImagenVilla(int idimagen);
        Task<List<MisReservas>> GetMisReservas(int idusuario);
        Task<List<Provincias>> GetProvincias();
        Task<List<Reserva>> GetReservasByIdVillaAsync(int idVilla);
        Task<List<MisReservas>> GetReservasByIdVillaVistaAsync(int idVilla);
        Task<List<Imagen>> GetImagenesVilla(int idvilla);
        Task<List<Villa>> GetVillasUnicasAsync();
        Task EditVilla(VillaTabla villa);
        Task<Imagen> InsertarImagenes(int idVilla, string url);
        Task<bool> CheckFechasDisponibles(int idVilla, DateTime fechaInicio, DateTime fechaFin);
        Task RegisterUser(string nombre, string email, string password, string telefono, int idrol);
        Task<Usuario> FindUsuarioEmailPassword(string email, string password);
        Task<string> GetTokenAsync(string username
           , string password);
    }
}
