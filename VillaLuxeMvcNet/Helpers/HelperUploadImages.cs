namespace VillaLuxeMvcNet.Helpers
{
    public class HelperUploadImages
    {
            private HelperPathProvider helperPathProvider;

            public HelperUploadImages(HelperPathProvider helperPathProvider)
            {
            this.helperPathProvider = helperPathProvider;
            }

             public async Task<List<string>> UploadFileAsync
        (List<IFormFile> files, Folders folder)
        {
            List<string> paths = new List<string>();
            foreach (IFormFile file in files)
            {
                string fileName = file.FileName;
                string path =
                    this.helperPathProvider.MapPath(fileName, folder);
                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                paths.Add(path);
            }
            return paths;
        }
    }
}
