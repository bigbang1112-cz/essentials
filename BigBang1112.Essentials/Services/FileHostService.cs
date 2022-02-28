using Microsoft.AspNetCore.Hosting;

namespace BigBang1112.Services;

public class FileHostService : IFileHostService
{
    private readonly IWebHostEnvironment _env;

    public FileHostService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public string GetFilePath(string folder, string fileName)
    {
#if DEBUG
        var saveDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folder);
#else
        var saveDir = Path.Combine(_env.ContentRootPath, folder);
#endif
        Directory.CreateDirectory(saveDir);

        var saveFile = Path.Combine(saveDir, fileName);

        return saveFile;
    }

    public string GetContentRootPath()
    {
        return _env.ContentRootPath;
    }
}
