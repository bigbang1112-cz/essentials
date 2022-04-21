using BigBang1112.Converters.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using System.IO.Compression;
using System.Text.Json;

namespace BigBang1112.Services;

public class FileHostService : IFileHostService
{
    private const string JsonExtension = "json.gz";
    
    private readonly IWebHostEnvironment _env;
    
    private static readonly JsonSerializerOptions jsonSerializerOptions = new(JsonSerializerDefaults.Web);

    static FileHostService()
    {
        jsonSerializerOptions.Converters.Add(new TimeInt32Converter());
    }

    public FileHostService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public string GetClosedFilePath(string folder, string fileName)
    {
#if DEBUG
        var saveDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folder);
#else
        var saveDir = Path.Combine(GetContentRootPath(), folder);
#endif
        Directory.CreateDirectory(saveDir);

        var saveFile = Path.Combine(saveDir, fileName);

        return saveFile;
    }

    public string GetContentRootPath()
    {
        return _env.ContentRootPath;
    }

    public string GetWebRootPath()
    {
        return _env.WebRootPath;
    }

    public IFileInfo GetFileInfo(string subpath)
    {
        return _env.WebRootFileProvider.GetFileInfo(subpath);
    }

    public void SaveToApi<T>(T jsonObject, int apiVersion, string fullFileNameWithoutExtension)
    {
        using var gzip = OpenStream_SaveJsonObjectToApi(apiVersion, fullFileNameWithoutExtension);
        JsonSerializer.Serialize(gzip, jsonObject, jsonSerializerOptions);
    }

    public async Task SaveToApiAsync<T>(T jsonObject, int apiVersion, string fullFileNameWithoutExtension, CancellationToken cancellationToken = default)
    {
        using var gzip = OpenStream_SaveJsonObjectToApi(apiVersion, fullFileNameWithoutExtension);
        await JsonSerializer.SerializeAsync(gzip, jsonObject, jsonSerializerOptions, cancellationToken);
    }

    private GZipStream OpenStream_SaveJsonObjectToApi(int apiVersion, string fullFileNameWithoutExtension)
    {
        var finalPath = GetOrCreateApiPath(apiVersion, fullFileNameWithoutExtension, JsonExtension);
        return new GZipStream(File.Create(finalPath), CompressionMode.Compress);
    }

    public void SaveToApi(Stream stream, int apiVersion, string fullFileNameWithExtension)
    {
        using var fs = OpenStream_SaveStreamToApi(apiVersion, fullFileNameWithExtension);
        stream.CopyTo(fs);
    }

    public async Task SaveToApiAsync(Stream stream, int apiVersion, string fullFileNameWithExtension, CancellationToken cancellationToken = default)
    {
        using var fs = OpenStream_SaveStreamToApi(apiVersion, fullFileNameWithExtension);
        await stream.CopyToAsync(fs, cancellationToken);
    }

    private FileStream OpenStream_SaveStreamToApi(int apiVersion, string fullFileNameWithExtension)
    {
        var finalPath = GetOrCreateApiPath(apiVersion, fullFileNameWithExtension);
        return File.Create(finalPath);
    }

    public string GetApiPath(int apiVersion, string fullFileNameWithoutExtension, string extension)
    {
        return Path.Combine(GetWebRootPath(), "api", $"v{apiVersion}", $"{fullFileNameWithoutExtension}.{extension}");
    }

    public string GetApiPath(int apiVersion, string fullFileNameWithExtension)
    {
        return Path.Combine(GetWebRootPath(), "api", $"v{apiVersion}", fullFileNameWithExtension);
    }

    public string GetOrCreateApiPath(int apiVersion, string fullFileNameWithoutExtension, string extension)
    {
        var path = GetApiPath(apiVersion, fullFileNameWithoutExtension, extension);
        var directoryName = Path.GetDirectoryName(path);

        if (directoryName is not null)
        {
            Directory.CreateDirectory(directoryName);
        }

        return path;
    }

    public string GetOrCreateApiPath(int apiVersion, string fullFileNameWithExtension)
    {
        var path = GetApiPath(apiVersion, fullFileNameWithExtension);
        var directoryName = Path.GetDirectoryName(path);

        if (directoryName is not null)
        {
            Directory.CreateDirectory(directoryName);
        }

        return path;
    }

    public bool ExistsInApi(int apiVersion, string fullFileNameWithoutExtension, string extension)
    {
        return File.Exists(GetApiPath(apiVersion, fullFileNameWithoutExtension, extension));
    }

    public bool JsonExistsInApi(int apiVersion, string fullFileNameWithoutExtension)
    {
        return File.Exists(GetApiPath(apiVersion, fullFileNameWithoutExtension, JsonExtension));
    }

    public T? GetFromApi<T>(int apiVersion, string fullFileNameWithoutExtension)
    {
        using var gzip = OpenStream_GetJsonObjectFromApi(apiVersion, fullFileNameWithoutExtension);

        if (gzip is null)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(gzip, jsonSerializerOptions);
    }

    public async Task<T?> GetFromApiAsync<T>(int apiVersion, string fullFileNameWithoutExtension, CancellationToken cancellationToken = default)
    {
        using var gzip = OpenStream_GetJsonObjectFromApi(apiVersion, fullFileNameWithoutExtension);

        if (gzip is null)
        {
            return default;
        }

        return await JsonSerializer.DeserializeAsync<T>(gzip, jsonSerializerOptions, cancellationToken);
    }

    private GZipStream? OpenStream_GetJsonObjectFromApi(int apiVersion, string fullFileNameWithoutExtension)
    {
        var finalPath = GetApiPath(apiVersion, fullFileNameWithoutExtension, JsonExtension);
        
        if (File.Exists(finalPath))
        {
            return new GZipStream(File.OpenRead(finalPath), CompressionMode.Decompress);
        }

        return null;
    }

    public DateTimeOffset? GetLastModifiedTimeFromApi(int apiVersion, string fullFileNameWithoutExtension)
    {
        var finalPath = GetApiPath(apiVersion, fullFileNameWithoutExtension, JsonExtension);

        if (File.Exists(finalPath))
        {
            return File.GetLastWriteTimeUtc(finalPath);
        }

        return default;
    }
}
