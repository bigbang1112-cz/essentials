using Microsoft.Extensions.FileProviders;

namespace BigBang1112.Services;

public interface IFileHostService
{
    IFileInfo GetFileInfo(string subpath);
    string GetFilePath(string folder, string fileName);
    string GetWebRootPath();
}
