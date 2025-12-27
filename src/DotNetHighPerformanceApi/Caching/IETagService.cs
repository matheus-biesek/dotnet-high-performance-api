using System;

namespace DotNetHighPerformanceApi.Caching;

public interface IETagService
{
    string GenerateETag(string entityType, int entityId, int version);
    bool ValidateETag(string eTag, string entityType, int entityId, int version);
}
