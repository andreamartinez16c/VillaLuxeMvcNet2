using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using VillaLuxeMvcNet.Models;
using VillaLuxeMvcNet.Repositories;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using Microsoft.EntityFrameworkCore;
using VillaLuxeMvcNet.Helpers;
using Azure.Storage.Blobs.Models;

namespace VillaLuxeMvcNet.Services
{
    public class ServiceVillas : IRepositoryVillas 
    {
        private string UrlApi;
        private MediaTypeWithQualityHeaderValue Header;
        private BlobServiceClient client;    
        public ServiceVillas(IConfiguration configuration, BlobServiceClient client)
        {
            this.Header =new MediaTypeWithQualityHeaderValue("application/json");
            this.UrlApi = configuration.GetValue<string>("ApiUrls:ApiVillaLuxe");
            this.client = client;
        }

        public async Task<string> GetTokenAsync(string username
            , string password)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/auth/login";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                LoginModel model = new LoginModel
                {
                    UserName = username,
                    Password = password
                };
                string jsonData = JsonConvert.SerializeObject(model);
                StringContent content =
                    new StringContent(jsonData, Encoding.UTF8,
                    "application/json");
                HttpResponseMessage response = await
                    client.PostAsync(request, content);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    JObject keys = JObject.Parse(data);
                    string token = keys.GetValue("response").ToString();
                    return token;
                }
                else
                {
                    return null;
                }
            }
        }

        private async Task<T> CallApiAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                HttpResponseMessage response =
                    await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        //TENDREMOS UN METODO GENERICO QUE RECIBIRA EL REQUEST 
        //Y EL TOKEN
        private async Task<T> CallApiAsync<T>
            (string request, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add
                    ("Authorization", "bearer " + token);
                HttpResponseMessage response =
                    await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        public Task<bool> CheckFechasDisponibles(int idVilla, DateTime fechaInicio, DateTime fechaFin)
        {
            throw new NotImplementedException();
        }

        public async Task CreateReserva(Reserva reserva)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/villas/insertreserva";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                string json = JsonConvert.SerializeObject(reserva);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
                
            }
        }

        public async Task<List<BlobModel>>
            GetBlobsAsync(string containerName)
        {
            //RECUPERAMOS EL CLIENT DEL CONTAINER
            BlobContainerClient containerClient =
                this.client.GetBlobContainerClient(containerName);
            List<BlobModel> models = new List<BlobModel>();
            await foreach (BlobItem item in
                containerClient.GetBlobsAsync())
            {
                //name, containerName, Url
                //DEBEMOS CREAR UN BLOBCLIENT SI NECESITAMOS 
                //TENER MAS INFORMACION DEL BLOB
                BlobClient blobClient =
                    containerClient.GetBlobClient(item.Name);
                BlobModel blob = new BlobModel();
                blob.Nombre = item.Name;
                blob.Contenedor = containerName;
                blob.Url = blobClient.Uri.AbsoluteUri;
                models.Add(blob);
            }
            return models;
        }


        public async Task UploadImageToBlobStorageAsync(string containerName, string blobName, Stream stream)
        {
            BlobContainerClient containerClient = this.client.GetBlobContainerClient(containerName);
            bool existe = false;
            foreach (var item in await this.GetBlobsAsync(containerName))
            {
                if (item.Nombre==blobName)
                {
                    existe = true;
                }
            }
            if (!existe)
            {
                    await containerClient.UploadBlobAsync(blobName, stream);

            }
        }

        //public async Task<string> UploadImageAsync(IFormFile image, string containerName)
        //{
        //    // Crear un contenedor si no existe
        //    var containerClient = client.GetBlobContainerClient(containerName);
        //    await containerClient.CreateIfNotExistsAsync();

        //    // Crear un blob client para la imagen
        //    var blobClient = containerClient.GetBlobClient(image.FileName);

        //    // Subir la imagen
        //    using (var stream = image.OpenReadStream())
        //    {
        //        await blobClient.UploadAsync(stream, overwrite: true);
        //    }

        //    // Devolver la URL de la imagen
        //    return blobClient.Uri.AbsoluteUri;
        //}
        public async Task<VillaTabla> CreateVillaAsync(VillaTabla villa)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/villas/insertvilla";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                string json = JsonConvert.SerializeObject(villa);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsAsync<VillaTabla>();
                    return data;
                }
            }

            return null;
        }

        public async Task DeleteImagenes(int idimagen)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/Villas/imagen/" + idimagen;
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                HttpResponseMessage response = await client.DeleteAsync(request);
            }
        }

        public async Task DeleteImagenesName(string imagen, int idvilla)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/Villas/imagen/" + imagen + "/" + idvilla;
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                HttpResponseMessage response = await client.DeleteAsync(request);
            }
        }

        public async Task DeleteReserva(int idReserva)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/villas/" + idReserva;
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                HttpResponseMessage response = await client.DeleteAsync(request);
            }
        }

        public async Task DeleteVilla(int idVilla)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/villas/deletevilla/" + idVilla;
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                HttpResponseMessage response = await client.DeleteAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    return;
                }
            }
        }

        public async Task EditVilla(VillaTabla villa)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/villas/editvilla";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                var content = new StringContent(JsonConvert.SerializeObject(villa), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync(request, content);
            }
        }


        //METODO PARA ELIMINAR UN BLOB DE UN CONTAINER
        public async Task DeleteBlobAsync(string containerName
            , string blobName)
        {
            BlobContainerClient containerClient =
                this.client.GetBlobContainerClient(containerName);
            await containerClient.DeleteBlobAsync(blobName);
        }


        public async Task<Imagen> FindImagenVilla(int idimagen)
        {
            string request = "api/Villas/imagen/" + idimagen;
            return await this.CallApiAsync<Imagen>(request);
        }

        public async Task<Reserva> FindReserva(int idReserva)
        {
            string request = "api/villas/detalles/" + idReserva;
            Reserva data = await this.CallApiAsync<Reserva>(request);
            return data;
        }

        public async Task<Villa> FindVillaAsync(int idvilla)
        {
            string request = "api/Villas/detallesVilla/" + idvilla;
            Villa data = await this.CallApiAsync<Villa>(request);
            return data;
        }

        public async Task<VillaFechasResevadas> FindVillaFechaReservadasAsync(int idvilla)
        {
            string request = "api/villas/" + idvilla;
            VillaFechasResevadas data = await this.CallApiAsync<VillaFechasResevadas>(request);
            return data;
        }

        public async Task<VillaTabla> FindVillaTAsync(int idvilla)
        {
            string request = "api/villas/" + idvilla;
            VillaTabla data = await this.CallApiAsync<VillaTabla>(request);
            return data;
        }

        

        public async Task<List<Imagen>> GetImagenesVilla(int idvilla)
        {
            string request = "api/Villas/imagenesvilla/" + idvilla;
            return await this.CallApiAsync<List<Imagen>>(request);
        }

        public async Task<List<MisReservas>> GetMisReservas(int idusuario)
        {
            string request = "api/Villas/usuario/" + idusuario;
            List<MisReservas> data = await this.CallApiAsync<List<MisReservas>>(request);
            return data;
        }

        public async Task<List<Provincias>> GetProvincias()
        {
            string request = "api/villas/provincias";
            List<Provincias> data = await this.CallApiAsync<List<Provincias>>(request);
            return data;
        }

        public async Task<List<Reserva>> GetReservasByIdVillaAsync(int idVilla)
        {
            string request = "api/villas/reservasvilla/" + idVilla;
            List<Reserva> data = await this.CallApiAsync<List<Reserva>>(request);
            return data;
        }

        public async Task<List<Villa>> GetVillasUnicasAsync()
        {
            string request = "api/villas/villasunicas";
            List<Villa> data = await this.CallApiAsync<List<Villa>>(request);
            return data;
        }

        public async Task<Imagen> InsertarImagenes(int idVilla, string url)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/Villas/insertimagen/" + idVilla + "/" + url;
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                HttpResponseMessage response = await client.PostAsync(request, null);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsAsync<Imagen>();
                    return data;
                }
            }

            return null;
        }


        public async Task<List<MisReservas>> GetReservasByIdVillaVistaAsync(int idVilla)
        {
            string request = "api/villas/reservasvilla/" + idVilla;
            List<MisReservas> data = await this.CallApiAsync<List<MisReservas>>(request);
            return data;
        }

        public async Task<Usuario> FindUsuarioEmailPassword(string email, string password)
        {
            string request = "api/Usuario/findUser/" + email + "/" + password;
            return await this.CallApiAsync<Usuario>(request);
        }

        
        private StringContent GetContentModel<T>(T data)
        {
            string jsonModel = JsonConvert.SerializeObject(data);
            StringContent content =
                new StringContent
                (jsonModel, Encoding.UTF8, "application/json");
            return content;
        }

        public async Task RegisterUser(RegisterModel model)
        {
            using (HttpClient client = new HttpClient())
            {
                model.Usuario.Contrasenia = new byte[] { };
                model.Usuario.Salt = "";
                string request = "api/Usuario/register";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                string json = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
                if(response.IsSuccessStatusCode)
                {
                    return;
                }
                else { 
                }
            }
        }
    }
}
