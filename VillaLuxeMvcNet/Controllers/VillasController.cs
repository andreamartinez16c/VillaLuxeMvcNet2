using Microsoft.AspNetCore.Mvc;
using VillaLuxeMvcNet.Models;
using VillaLuxeMvcNet.Repositories;

namespace VillaLuxeMvcNet.Controllers
{
    public class VillasController : Controller
    {
        private RepositoryVillas repo;
        public VillasController(RepositoryVillas repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            List<Villa> villas = await this.repo.GetVillasUnicasAsync();
            return View(villas);
        }

        public async Task<IActionResult> DetallesVilla(int idvilla)
        {

            await this.repo.GetFechasReservadasByIdVillaAsync(idvilla);
           Villa villa = await this.repo.FindVillaAsync(idvilla);
            return View(villa);
        }

        //----------------RESERVAS---------------

        [HttpPost]
        public async Task<IActionResult> DetallesVilla(Reserva reserva)
        {
            try
            {
                int idusuario = HttpContext.Session.GetInt32("IDUSUARIO") ?? 0;

                await this.repo.CreateReserva(reserva, idusuario);
                // Configurar un mensaje de confirmación en TempData
                TempData["ReservaConfirmada"] = "¡Reserva realizada con éxito!";

                // Redirigir de vuelta a la vista DetallesVilla
                return RedirectToAction("DetallesVilla", new { idvilla = reserva.IdVilla });
            }
            catch (Exception ex)
            {
                // Configurar un mensaje de error en TempData
                TempData["ErrorReserva"] = ex.Message;

                // Redirigir de vuelta a la vista DetallesVilla
                return RedirectToAction("DetallesVilla", new { idvilla = reserva.IdVilla });
            }
        }

        public async Task<IActionResult> MisReservas()
        {
            int idusuario = HttpContext.Session.GetInt32("IDUSUARIO") ?? 0;
            var reservas = await this.repo.GetMisReservas(idusuario);
            return View(reservas);
        }

        public async Task<IActionResult> DeleteReserva(int idreserva)
        {
            await this.repo.DeleteReserva(idreserva);
            return RedirectToAction("MisReservas");
        }
    }
}
