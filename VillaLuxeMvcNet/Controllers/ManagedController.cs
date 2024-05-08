using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VillaLuxeMvcNet.Models;
using VillaLuxeMvcNet.Services;
using VillaLuxeMvcNet.Repositories;
using Microsoft.AspNetCore.Identity;

namespace VillaLuxeMvcNet.Controllers
{
    public class ManagedController : Controller
    {
        private IRepositoryVillas service;
        public ManagedController(IRepositoryVillas service)
        {
            this.service = service;      
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Usuario usu, string password)
        {
            RegisterModel register = new RegisterModel();
            register.Usuario = usu;
            register.Password = password;
            await this.service.RegisterUser(register);                                  
            TempData["RegistroExitoso"] = "Usuario registrado";
            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            ViewData["MENSAJE"] = TempData["RegistroExitoso"]; // Pasar el mensaje de TempData a ViewData
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login
     (string email, string password)
        {
            Usuario user = await
                this.service.FindUsuarioEmailPassword(email, password);
            string token = await this.service
                .GetTokenAsync(email, password);
            if (user != null)
            {
                //SEGURIDAD
                ClaimsIdentity identity =
                    new ClaimsIdentity(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        ClaimTypes.Name, ClaimTypes.Role);
                Claim claimName =
                    new Claim(ClaimTypes.Name, user.Nombre);
                Claim claimRole = new Claim(ClaimTypes.Role, user.IdRol.ToString());
                Claim claimIdUsuario = new Claim("IDUSUARIO", user.IdUsuario.ToString());
                Claim claimToken = new Claim("TOKEN", token);
                identity.AddClaim(claimRole);
                identity.AddClaim(claimName);
                identity.AddClaim(claimIdUsuario);
                identity.AddClaim(claimToken);
                ClaimsPrincipal userPrincipal =
                    new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    userPrincipal);
                TempData["BIENVENIDA"] = "Bienvenido " + user.Nombre;
                return RedirectToAction("Index", "Villas");
            }
            else
            {
                TempData["MENSAJE"] = "Credenciales incorrectas";
                return View();
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync
                (CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Villas");
        }

    }
}
