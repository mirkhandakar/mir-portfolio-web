namespace MirPortfolio.Web.Services;

public interface ISitemapService
{
    Task<string> ToXmlAsync(Uri baseUrl, CancellationToken cancellationToken = default);
}
