using Microsoft.AspNetCore.Mvc;
using VillaLuxeMvcNet.Helpers;
using VillaLuxeMvcNet.Models;
using VillaLuxeMvcNet.Repositories;
using static System.Net.Mime.MediaTypeNames;

namespace VillaLuxeMvcNet.Controllers
{
    public class VillasController : Controller
    {
        private RepositoryVillas repo;
        private HelperPathProvider helperPathProvider;
        public VillasController(RepositoryVillas repo, HelperPathProvider helperPathProvider)
        {
            this.repo = repo;
            this.helperPathProvider = helperPathProvider;
        }

        public async Task<IActionResult> Index()
        {
            List<Villa> villas = await this.repo.GetVillasUnicasAsync();
            return View(villas);
        }

        public async Task<IActionResult> DetallesVilla(int idvilla)
        {
            VillaFechasResevadas fechasReservadas= new VillaFechasResevadas();
          fechasReservadas.FechasReservadas= await this.repo.GetFechasReservadasByIdVillaAsync(idvilla);
           fechasReservadas.Villa = await this.repo.FindVillaAsync(idvilla);
            return View(fechasReservadas);
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
                /*TempData["ReservaConfirmada"] = "¡Reserva realizada con éxito!";*/

                // Redirigir de vuelta a la vista DetallesVilla
                return RedirectToAction("PagoReserva", new { idvilla = reserva.IdVilla });
            }
            catch (Exception ex)
            {
                // Configurar un mensaje de error en TempData
                TempData["ErrorReserva"] = ex.Message;

                // Redirigir de vuelta a la vista DetallesVilla
                return RedirectToAction("DetallesVilla", new { idvilla = reserva.IdVilla });
            }
        }

        public async Task<IActionResult> PagoReserva(int idvilla)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PagoReserva(Reserva reserva)
        {
            TempData["ReservaConfirmada"] = "¡Reserva realizada con éxito!";

            // Redirigir de vuelta a la vista DetallesVilla
            return RedirectToAction("MisReservas");
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
        public async Task<IActionResult> DeleteVilla(int idvilla)
        {
            List<Imagen> imagenes = await this.repo.GetImagenesVilla(idvilla);
            foreach(var imagen in imagenes)
            {
                await this.repo.DeleteImagenes(imagen.IdImagen);
            }
            await this.repo.DeleteVilla(idvilla);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AllReservasVilla(int idvilla)
        {
            // Obtener la lista de reservas
            var reservas = await this.repo.GetReservasByIdVillaVistaAsync(idvilla);

            // Convertir las reservas a MisReservas
            List<MisReservas> misReservas = reservas.Select(r => new MisReservas
            {
                IdReserva = r.IdReserva,
                IdUsuario = r.IdUsuario,
                IdVilla = r.IdVilla,
                FechaInicio = r.FechaInicio,
                FechaFin = r.FechaFin,
                PrecioTotal = r.PrecioTotal,
                Estado = r.Estado,
                Nombre = r.Nombre,
                Direccion = r.Direccion,
                Ubicacion = r.Ubicacion,
                Imagen = r.Imagen
            }).ToList();

            // Pasar las MisReservas a la vista
            return View(misReservas);
        }

        public async Task<IActionResult> CreateVilla()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateVilla(VillaTabla villa, List<IFormFile> imagen, IFormFile imagenCollage)
        {
            villa.ImagenCollage = imagenCollage.FileName;
            VillaTabla villaT = await this.repo.CreateVillaAsync(villa);
           

            foreach( IFormFile img in imagen)
            {
                string path =
               this.helperPathProvider.MapPath
               (img.FileName, Folders.Images);
                //SUBIMOS EL FICHERO UTILIZANDO Stream
                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    //MEDIANTE IFormFile COPIAMOS EL CONTENIDO DEL FICHERO
                    //AL STREAM
                    await img.CopyToAsync(stream);
                }
                
                string urlPath =
                    this.helperPathProvider.MapUrlPath(img.FileName
                    , Folders.Images);
                await this.repo.InsertarImagenes(villaT.IdVilla, urlPath);
            }

            string pathBanner =
              this.helperPathProvider.MapPath
              (imagenCollage.FileName, Folders.Images);
            //SUBIMOS EL FICHERO UTILIZANDO Stream
            using (Stream stream = new FileStream(pathBanner, FileMode.Create))
            {
                //MEDIANTE IFormFile COPIAMOS EL CONTENIDO DEL FICHERO
                //AL STREAM
                await imagenCollage.CopyToAsync(stream);
            }

            string urlPathBanner =
                this.helperPathProvider.MapUrlPath(imagenCollage.FileName
                , Folders.Images);
            await this.repo.InsertarImagenes(villaT.IdVilla, urlPathBanner);

            TempData["VillaCreada"] = "¡Villa Insertada con éxito!";

            // Redirigir de vuelta a la vista DetallesVilla
            return RedirectToAction("Index");
        }



        public async Task<IActionResult> EditVillas(int idvilla)
        {
            Villa villa = await this.repo.FindVillaAsync(idvilla);
            return View(villa);
    }

        [HttpPost]
        public async Task<IActionResult> EditVillas(VillaTabla villa)
        {
            try
            {
                await this.repo.EditVilla(villa);
                // Configurar un mensaje de confirmación en TempData
                TempData["VillaModificada"] = "¡Villa Modificada con éxito!";

                // Redirigir de vuelta a la vista DetallesVilla
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Configurar un mensaje de error en TempData
                TempData["ErrorVilla"] = ex.Message;

                // Redirigir de vuelta a la vista DetallesVilla
                return RedirectToAction("Index");
            }

        }

        public async Task<IActionResult> DeleteImagenModificar(int idimagen, int idvilla)
        {
            await this.repo.DeleteImgVilla(idimagen);
            return RedirectToAction("EditVillas",  new { idvilla = idvilla });
        }
    }


}
