using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;

namespace VillaLuxeMvcNet.Helpers
{
    public enum Folders { Images = 0}
    public class HelperPathProvider
    {

        private IServer server;
            //NECESITAMOS ACCEDER AL SISTEMA DE ARCHIVOS DEL WEB SERVER (wwwroot)
            private IWebHostEnvironment hostEnvironment;

            public HelperPathProvider(IWebHostEnvironment hostEnvironment
                , IServer server)
            {
                this.server = server;
                this.hostEnvironment = hostEnvironment;
            }

            //CREAMOS UN METODO PRIVADO QUE NOS DEVUELVA EL 
            //NOMBRE DE LA CARPETA DEPENDIENDO DEL Folder
            private string GetFolderPath(Folders folder)
            {
                string carpeta = "";
                if (folder == Folders.Images)
                {
                    carpeta = "img";
                }
                return carpeta;
            }

            public string MapPath(string fileName, Folders folder)
            {
                string carpeta = this.GetFolderPath(folder);
                string rootPath = this.hostEnvironment.WebRootPath;
                string path = Path.Combine(rootPath, carpeta, fileName);
                return path;
            }

            public string MapUrlPath(string fileName, Folders folder)
            {
                string carpeta = this.GetFolderPath(folder);
                var addresses =
                    server.Features.Get<IServerAddressesFeature>().Addresses;
                string serverUrl = addresses.FirstOrDefault();
                string urlPath =  fileName;
                return urlPath;
            }
        }

}
