using System;
using System.Security.Cryptography;
using System.Text;

namespace DotNetHighPerformanceApi.Caching;

public class ETagService : IETagService
{
    public string GenerateETag(string entityType, int entityId, int version)
    {
        var data = $"{entityType}:{entityId}:{version}";
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
