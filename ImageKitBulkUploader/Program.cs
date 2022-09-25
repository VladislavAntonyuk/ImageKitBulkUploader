using Imagekit;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("config.json", false)
    .Build();

var baseDir = config.GetSection("imagePath").Value;
var imageKitConfig = config.GetSection("imageKit").Get<ImageKitConfig>();
var imagekit = new ServerImagekit(imageKitConfig.PublicKey, 
    imageKitConfig.PrivateKey, 
    imageKitConfig.UrlEndpoint,
    imageKitConfig.TransformationPosition);

ProcessDirectories(new DirectoryInfo(baseDir), imagekit);

void ProcessDirectories(DirectoryInfo dirInput, ServerImagekit serverImagekit)
{
    foreach (var di in dirInput.GetDirectories())
    {
        var directoryName = di.Name;
        if (!Directory.Exists(directoryName))
        {
            var files = Directory.GetFiles(di.FullName);
            foreach (var file in files)
            {
                var filePath = new FileInfo(file);
                serverImagekit.Folder(di.FullName.Replace(baseDir, ""))
                    .FileName(filePath.Name)
                    .UseUniqueFileName(false)
                    .Upload(file);
            }
        }

        ProcessDirectories(di, serverImagekit);
    }
}

public class ImageKitConfig
{
    public string? PublicKey { get; set; }
    public string? PrivateKey { get; set; }
    public string? UrlEndpoint { get; set; }
    public string? TransformationPosition { get; set; }
}