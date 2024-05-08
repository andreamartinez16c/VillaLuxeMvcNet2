using Microsoft.EntityFrameworkCore;
using VillaLuxeMvcNet.Data;
using VillaLuxeMvcNet.Helpers;
using VillaLuxeMvcNet.Models;

namespace VillaLuxeMvcNet.Repositories
{
    public class RepositotyUsuarios: IRepositoryUsuarios
    {
        private VillaContext context;
        public RepositotyUsuarios(VillaContext context)
        {
            this.context = context;
        }

        private async Task<int> GetMaxIdUsuarioAsync()
        {
            if (this.context.Usuarios.Count() == 0)
            {
                return 1;
            }
            else
            {
                return await
                    this.context.Usuarios.MaxAsync(z => z.IdUsuario) + 1;
            }
        }

        public async Task RegisterUser(string nombre, string email
            , string password, string telefono, int idrol)
        {
            Usuario user = new Usuario();
            user.IdUsuario = await this.GetMaxIdUsuarioAsync();
            user.Nombre = nombre;
            user.Email = email;
            user.Telefono = telefono; 
            user.IdRol = 1;
            //CADA USUARIO TENDRA UN SALT DISTINTO
            user.Salt = HelperCryptography.GenerateSalt();
            //GUARDAMOS EL PASSWORD EN BYTE[]
            user.Contrasenia =
                HelperCryptography.EncryptPassword(password, user.Salt);
            this.context.Usuarios.Add(user);
            await this.context.SaveChangesAsync();
        }

        public async Task<Usuario> LogInUserAsync(string email, string password)
        {
            Usuario user = await
                this.context.Usuarios.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null)
            {
                return null;
            }
            else
            {
                string salt = user.Salt;
                byte[] temp =
                    HelperCryptography.EncryptPassword(password, salt);
                byte[] passUser = user.Contrasenia;
                bool response =
                    HelperCryptography.CompareArrays(temp, passUser);
                if (response == true)
                {
                    return user;
                }
                else
                {
                    return null;
                }
            }
        }

    }
}
