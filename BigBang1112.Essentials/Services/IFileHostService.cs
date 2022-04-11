using Microsoft.Extensions.FileProviders;

namespace BigBang1112.Services;

public interface IFileHostService
{
    IFileInfo GetFileInfo(string subpath);
    string GetFilePath(string folder, string fileName);
    string GetWebRootPath();

    string GetDirectoryName(string fullFileName)
    {
        return Path.GetDirectoryName(fullFileName) ?? throw new Exception();
    }

    void SaveToApi<T>(T jsonObject, int apiVersion, string fullFileNameWithoutExtension);
    Task SaveToApiAsync<T>(T jsonObject, int apiVersion, string fullFileNameWithoutExtension, CancellationToken cancellationToken = default);
    void SaveToApi(Stream stream, int apiVersion, string fullFileNameWithExtension);
    Task SaveToApiAsync(Stream stream, int apiVersion, string fullFileNameWithExtension, CancellationToken cancellationToken = default);
    string GetApiPath(int apiVersion, string fullFileNameWithoutExtension, string extension);
}
