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


        public async Task<IActionResult> DetallesVilla(int idvilla)
        {
            VillaFechasResevadas fechasReservadas = await this.service.FindVillaFechaReservadasAsync(idvilla);

            return View(fechasReservadas);
        }

        //----------------RESERVAS---------------

        [AuthorizeUsuarios]
        [HttpPost]
        public async Task<IActionResult> DetallesVilla(Reserva reserva)
        {
            try
            {
                int idusuario = int.Parse(User.FindFirst("IDUSUARIO").Value);
                reserva.IdUsuario = idusuario;
                reserva.Estado = "EN PROCESO";
                await this.service.CreateReserva(reserva);
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
            int idusuario = int.Parse(User.FindFirst("IDUSUARIO").Value);
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
            foreach (var imagen in imagenes)
            {
                await this.service.DeleteBlobAsync("villasimagenes", imagen.Imgn);
                await this.service.DeleteImagenes(imagen.IdImagen);
            }
            await this.service.DeleteVilla(idvilla);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AllReservasVilla(int idvilla)
        {
            // Obtener la lista de reservas
            var reservas = await this.service.GetReservasByIdVillaVistaAsync(idvilla);

            if (reservas == null)
            {
                return View();
            }
            else
            {
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

        }

        public IActionResult CreateVilla()
        {
            return View();
        }

        /*[HttpPost]
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
        }*/
        /*
                public async Task<IActionResult> DeleteImagenModificarBlobs(string containername, string blobname, int idvilla)
                {
                    await this.service.DeleteBlobAsync(containername, blobname);
                    return RedirectToAction("EditVillas", new { idvilla = idvilla });
                }*/

        [HttpPost]
        public async Task<IActionResult> EditVillas(VillaTabla villa, List<IFormFile> imagen)
        {
            await this.service.EditVilla(villa);


            foreach (IFormFile file in imagen)
            {
                string blobName = file.FileName;
                using (Stream stream = file.OpenReadStream())
                {
                    await this.service.DeleteBlobAsync("villasimagenes", blobName);

                    await this.service
                        .UploadImageToBlobStorageAsync("villasimagenes", blobName, stream);
                }
                await this.service.DeleteImagenesName(blobName, villa.IdVilla);
                await this.service.InsertarImagenes(villa.IdVilla, blobName);
            }

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

        [HttpPost]
        public async Task<IActionResult> CreateVilla(VillaTabla villa, List<IFormFile> imagen, IFormFile imagenCollage)
        {
            string blobNameCollage = imagenCollage.FileName;
            villa.ImagenCollage = blobNameCollage;
            VillaTabla villaT = await this.service.CreateVillaAsync(villa);
            using (Stream stream = imagenCollage.OpenReadStream())
            {
                await this.service
                    .UploadImageToBlobStorageAsync("villasimagenes", blobNameCollage, stream);
            }


            foreach (IFormFile file in imagen)
            {
                string blobName = file.FileName;
                using (Stream stream = file.OpenReadStream())
                {
                    await this.service
                        .UploadImageToBlobStorageAsync("villasimagenes", blobName, stream);
                }
                await this.service.InsertarImagenes(villaT.IdVilla, blobName);

            }

            //string urlPath = await this.service.UploadImageAsync(imagen, "villasimagenes");
            //await this.repo.InsertarImagenes(villaT.IdVilla, urlPath);


            TempData["VillaCreada"] = "¡Villa Insertada con éxito!";

            // Redirigir de vuelta a la vista DetallesVilla
            return RedirectToAction("Index");
        }



        public async Task<IActionResult> EditVillas(int idvilla)
        {
            Villa villa = await this.service.FindVillaAsync(idvilla);
            return View(villa);
        }

        /*[HttpPost]
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

        }*/

        public async Task<IActionResult> DeleteImagenModificar(int idimagen, int idvilla, string imagenname)
        {
            await this.service.DeleteImagenes(idimagen);
            await this.service.DeleteBlobAsync("villasimagenes", imagenname);
            return RedirectToAction("EditVillas", new { idvilla = idvilla });
        }



    }


}
