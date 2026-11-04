namespace MirPortfolio.Web.Services;

public record ResumeUploadResult(bool Succeeded, string? RelativePath, string? Error);

public interface IResumeStorageService
{
    Task<ResumeUploadResult> SaveAsync(IFormFile file, CancellationToken cancellationToken = default);

    string? GetPhysicalPath(string? relativePath);

    bool Exists(string? relativePath);

    void Delete(string? relativePath);
}
