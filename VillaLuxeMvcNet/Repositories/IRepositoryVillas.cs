using VillaLuxeMvcNet.Models;

namespace VillaLuxeMvcNet.Repositories
{
    public interface IRepositoryVillas
    {
        Task<VillaTabla> CreateVillaAsync(VillaTabla villa);
        Task CreateReserva(Reserva reserva);
        Task DeleteImagenes(int idimagen);
        Task DeleteImagenesName(string imagen, int idvilla);
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
        Task RegisterUser(RegisterModel model);
        Task<Usuario> FindUsuarioEmailPassword(string email, string password);
        Task<string> GetTokenAsync(string username
           , string password);





        Task UploadImageToBlobStorageAsync(string containerName, string blobName, Stream stream);
        Task DeleteBlobAsync(string containerName, string blobName);
    }
}
