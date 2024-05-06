using VillaLuxeMvcNet.Models;

namespace VillaLuxeMvcNet.Repositories
{
    public interface IRepositoryUsuarios
    {
        Task RegisterUser(string nombre, string email, string password, string telefono, int idrol);
        Task<Usuario> LogInUserAsync(string email, string password);
    }
}
