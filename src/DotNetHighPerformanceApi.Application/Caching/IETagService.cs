namespace DotNetHighPerformanceApi.Application.Caching;

public interface IETagService
{
    string GenerateETag<T>(T data);
    string GenerateETag(string name, int id, int version);
    string GenerateETag(string name, int id, DateTimeOffset lastModified);
}
