using DotNetHighPerformanceApi.Application.Caching;
using System;
using System.Security.Cryptography;
using System.Text;

namespace DotNetHighPerformanceApi.Infrastructure.Caching;

public class ETagService : IETagService
{
    public string GenerateETag<T>(T data)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(data);
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(json));
        var hash = Convert.ToBase64String(hashBytes);
        return $"W/\"{hash}\"";
    }

    public string GenerateETag(string name, int id, int version)
    {
        var data = $"{name}:{id}:{version}";
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(data));
        var hash = Convert.ToBase64String(hashBytes);
        return $"W/\"{hash}\"";
    }
    
    public string GenerateETag(string name, int id, DateTimeOffset lastModified)
    {
         var data = $"{name}:{id}:{lastModified.ToUnixTimeMilliseconds()}";
         var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(data));
         var hash = Convert.ToBase64String(hashBytes);
         return $"W/\"{hash}\"";
    }

    public bool ValidateETag(string eTag, string entityType, int entityId, int version)
    {
        if (string.IsNullOrEmpty(eTag))
            return false;

        var currentETag = GenerateETag(entityType, entityId, version);
        return eTag.Equals(currentETag, StringComparison.Ordinal);
    }
}
