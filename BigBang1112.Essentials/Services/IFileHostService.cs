using Microsoft.Extensions.FileProviders;

namespace BigBang1112.Services;

public interface IFileHostService
{
    IFileInfo GetFileInfo(string subpath);
    string GetClosedFilePath(string folder, string fileName);
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
    string GetApiPath(int apiVersion, string fullFileNameWithExtension);
    string GetOrCreateApiPath(int apiVersion, string fullFileNameWithoutExtension, string extension);
    string GetOrCreateApiPath(int apiVersion, string fullFileNameWithExtension);
    bool ExistsInApi(int apiVersion, string fullFileNameWithoutExtension, string extension);
    bool JsonExistsInApi(int apiVersion, string fullFileNameWithoutExtension);
    T GetFromApi<T>(int apiVersion, string fullFileNameWithoutExtension);
    Task<T> GetFromApiAsync<T>(int apiVersion, string fullFileNameWithoutExtension, CancellationToken cancellationToken = default);
}
