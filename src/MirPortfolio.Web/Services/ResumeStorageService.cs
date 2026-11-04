using System.Text;

namespace MirPortfolio.Web.Services;

public class ResumeStorageService : IResumeStorageService
{
    public const long MaxFileSizeBytes = 5 * 1024 * 1024;
    private const string ResumeFolder = "resume";
    private const string PdfHeader = "%PDF-";

    private readonly IWebHostEnvironment _environment;

    public ResumeStorageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<ResumeUploadResult> SaveAsync(
        IFormFile file,
        CancellationToken cancellationToken = default)
    {
        if (file.Length == 0)
        {
            return new ResumeUploadResult(false, null, "Choose a PDF file to upload.");
        }

        if (file.Length > MaxFileSizeBytes)
        {
            return new ResumeUploadResult(false, null, "The resume must be 5 MB or smaller.");
        }

        var extension = Path.GetExtension(file.FileName);
        if (!string.Equals(extension, ".pdf", StringComparison.OrdinalIgnoreCase))
        {
            return new ResumeUploadResult(false, null, "Only PDF files are supported.");
        }

        await using (var stream = file.OpenReadStream())
        {
            var header = new byte[PdfHeader.Length];
            var bytesRead = await stream.ReadAtLeastAsync(
                header,
                header.Length,
                throwOnEndOfStream: false,
                cancellationToken: cancellationToken);

            if (bytesRead < header.Length
                || !string.Equals(Encoding.ASCII.GetString(header), PdfHeader, StringComparison.Ordinal))
            {
                return new ResumeUploadResult(false, null, "The file is not a valid PDF.");
            }
        }

        var directory = GetResumeDirectory();
        Directory.CreateDirectory(directory);

        var fileName = $"resume-{Guid.NewGuid():N}.pdf";
        var fullPath = Path.Combine(directory, fileName);

        try
        {
            await using var target = new FileStream(
                fullPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None);

            await using var source = file.OpenReadStream();
            await source.CopyToAsync(target, cancellationToken);
        }
        catch (IOException)
        {
            TryDelete(fullPath);
            return new ResumeUploadResult(false, null, "The upload could not be saved. Try again.");
        }
        catch (UnauthorizedAccessException)
        {
            TryDelete(fullPath);
            return new ResumeUploadResult(false, null, "The upload could not be saved. Check folder permissions.");
        }

        return new ResumeUploadResult(true, $"{ResumeFolder}/{fileName}", null);
    }

    public string? GetPhysicalPath(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return null;
        }

        var resumeDirectory = GetResumeDirectory();
        var candidate = Path.GetFullPath(
            Path.Combine(
                GetWebRootPath(),
                relativePath.Replace('/', Path.DirectorySeparatorChar)));

        var prefix = resumeDirectory + Path.DirectorySeparatorChar;

        return candidate.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
            ? candidate
            : null;
    }

    public bool Exists(string? relativePath)
    {
        var fullPath = GetPhysicalPath(relativePath);

        return fullPath is not null && File.Exists(fullPath);
    }

    public void Delete(string? relativePath)
    {
        var fullPath = GetPhysicalPath(relativePath);

        if (fullPath is not null)
        {
            TryDelete(fullPath);
        }
    }

    private string GetWebRootPath()
    {
        return string.IsNullOrWhiteSpace(_environment.WebRootPath)
            ? Path.Combine(_environment.ContentRootPath, "wwwroot")
            : _environment.WebRootPath;
    }

    private string GetResumeDirectory()
    {
        return Path.GetFullPath(Path.Combine(GetWebRootPath(), ResumeFolder));
    }

    private static void TryDelete(string fullPath)
    {
        try
        {
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}
