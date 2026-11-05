using MirPortfolio.Web.Models;

namespace MirPortfolio.Web.ViewModels;

public class BlogPostDetailViewModel
{
    public BlogPost Post { get; set; } = new();

    public string RenderedBody { get; set; } = string.Empty;
}
