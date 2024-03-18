using Microsoft.AspNetCore.Mvc;
using VillaLuxeMvcNet.Models;
using VillaLuxeMvcNet.Repositories;

namespace VillaLuxeMvcNet.Controllers
{
    public class UsuariosController : Controller
    {
        private RepositotyUsuarios repo;
        public UsuariosController(RepositotyUsuarios repo)
        {
            this.repo = repo;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string nombre, string email, string password, string telefono, int idrol)
        {
            await this.repo.RegisterUser(nombre, email, password, telefono, idrol);
            TempData["RegistroExitoso"] = "Usuario registrado"; // Cambia el nombre de la clave
            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            ViewData["MENSAJE"] = TempData["RegistroExitoso"]; // Pasar el mensaje de TempData a ViewData
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            Usuario user = await this.repo.LogInUserAsync(email, password);
            if (user == null)
            {
                TempData["MENSAJE"] = "Credenciales incorrectas";
                return View();
            }
            else
            {
                HttpContext.Session.SetInt32("IDUSUARIO", user.IdUsuario);
                TempData["BienvenidaUsuario"] = "Bienvenido " + user.Nombre; // Aquí asumimos que el nombre del usuario se encuentra en la propiedad Nombre del objeto Usuario
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
