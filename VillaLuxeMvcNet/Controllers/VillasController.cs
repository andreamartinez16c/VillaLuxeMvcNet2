using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;
using System.Security.Claims;
using VillaLuxeMvcNet.Filters;
using VillaLuxeMvcNet.Helpers;
using VillaLuxeMvcNet.Models;
using VillaLuxeMvcNet.Repositories;
using VillaLuxeMvcNet.Services;
using static System.Net.Mime.MediaTypeNames;
using System.Security.Claims;


namespace VillaLuxeMvcNet.Controllers
{
    public class VillasController : Controller
    {
        /*private RepositoryVillas repo;*/
        private IRepositoryVillas service;
        private HelperPathProvider helperPathProvider;
        public VillasController(IRepositoryVillas service, HelperPathProvider helperPathProvider)
        {
           /* this.repo = repo;*/
            this.service = service;
            this.helperPathProvider = helperPathProvider;
        }

        public async Task<IActionResult> Index()
        {
            List<Villa> villas = await this.service.GetVillasUnicasAsync();
            return View(villas);
        }

        [AuthorizeUsuarios]
        public async Task<IActionResult> DetallesVilla(int idvilla)
        {
            VillaFechasResevadas fechasReservadas= await this.service.FindVillaFechaReservadasAsync(idvilla);
            
            return View(fechasReservadas);
        }
        
        //----------------RESERVAS---------------

        [HttpPost]
        public async Task<IActionResult> DetallesVilla(Reserva reserva)
        {
            try
            {
                int idusuario = int.Parse(User.FindFirst("IDUSUARIO").Value);

                await this.service.CreateReserva(reserva, idusuario);
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

        public IActionResult PagoReserva(int idvilla)
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
            int idusuario= int.Parse(User.FindFirst("IDUSUARIO").Value);
            var reservas = await this.service.GetMisReservas(idusuario);
            return View(reservas);
        }

        public async Task<IActionResult> DeleteReserva(int idreserva)
        {
            await this.service.DeleteReserva(idreserva);
            return RedirectToAction("MisReservas");
        }
        public async Task<IActionResult> DeleteVilla(int idvilla)
        {
            List<Imagen> imagenes = await this.service.GetImagenesVilla(idvilla);
            foreach(var imagen in imagenes)
            {
                await this.service.DeleteImagenes(imagen.IdImagen);
            }
            await this.service.DeleteVilla(idvilla);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AllReservasVilla(int idvilla)
        {
            // Obtener la lista de reservas
            var reservas = await this.service.GetReservasByIdVillaVistaAsync(idvilla);

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

        public IActionResult CreateVilla()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateVilla(VillaTabla villa, List<IFormFile> imagen, IFormFile imagenCollage)
        {
            villa.ImagenCollage = imagenCollage.FileName;
            VillaTabla villaT = await this.service.CreateVillaAsync(villa);


            foreach (IFormFile img in imagen)
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

                //BLOBS, recoger imagen e insertarla
                await this.service.InsertarImagenes(villaT.IdVilla, urlPath);
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

            await this.service.InsertarImagenes(villaT.IdVilla, urlPathBanner);

            TempData["VillaCreada"] = "¡Villa Insertada con éxito!";

            // Redirigir de vuelta a la vista DetallesVilla
            return RedirectToAction("Index");
        }

        /* [HttpPost]
         public async Task<IActionResult> CreateVilla(VillaTabla villa, IFormFile imagen, IFormFile imagenCollage)
         {
             villa.ImagenCollage = await this.service.UploadImageAsync(imagenCollage, "villasimagenes");
             VillaTabla villaT = await this.service.CreateVillaAsync(villa);


             string urlPath = await this.service.UploadImageAsync(imagen, "villasimagenes");
             //await this.repo.InsertarImagenes(villaT.IdVilla, urlPath);


         TempData["VillaCreada"] = "¡Villa Insertada con éxito!";

             // Redirigir de vuelta a la vista DetallesVilla
             return RedirectToAction("Index");
         }*/



        public async Task<IActionResult> EditVillas(int idvilla)
        {
            Villa villa = await this.service.FindVillaAsync(idvilla);
            return View(villa);
    }

        [HttpPost]
        public async Task<IActionResult> EditVillas(VillaTabla villa)
        {
            try
            {
                await this.service.EditVilla(villa);
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
            await this.service.DeleteImagenes(idimagen);
            return RedirectToAction("EditVillas",  new { idvilla = idvilla });
        }
    }


}
