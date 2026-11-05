using Microsoft.EntityFrameworkCore;
using MirPortfolio.Web.Data;
using MirPortfolio.Web.Models;
using MirPortfolio.Web.Services;

namespace MirPortfolio.Web.Tests;

public class BlogPostServiceTests : ServiceTestBase
{
    [Fact]
    public async Task CreateAsync_ClearsPublishedAtForDraft()
    {
        var context = CreateContext();
        var service = new BlogPostService(context);

        var post = new BlogPost
        {
            Title = "Draft post",
            Slug = "draft-post",
            Excerpt = "Draft excerpt",
            Body = "Draft body",
            IsPublished = false,
            PublishedAt = DateTime.UtcNow
        };

        var created = await service.CreateAsync(post);

        Assert.False(created.IsPublished);
        Assert.Null(created.PublishedAt);
        Assert.Equal(DateTime.UtcNow.Date, created.CreatedAt.Date);
        Assert.Equal(created.CreatedAt.Date, created.UpdatedAt.Date);
    }

    [Fact]
    public async Task CreateAsync_SetsPublishedAtForPublishedPost()
    {
        var context = CreateContext();
        var service = new BlogPostService(context);

        var post = new BlogPost
        {
            Title = "Published post",
            Slug = "published-post",
            Excerpt = "Published excerpt",
            Body = "Published body",
            IsPublished = true,
            PublishedAt = null
        };

        var created = await service.CreateAsync(post);

        Assert.True(created.IsPublished);
        Assert.NotNull(created.PublishedAt);
        Assert.Equal(DateTime.UtcNow.Date, created.PublishedAt.Value.Date);
    }

    [Fact]
    public async Task UpdateAsync_ClearsPublishedAtWhenPostBecomesDraft()
    {
        var context = CreateContext();
        var service = new BlogPostService(context);
        var created = await service.CreateAsync(new BlogPost
        {
            Title = "Published post",
            Slug = "published-post",
            Excerpt = "Published excerpt",
            Body = "Published body",
            IsPublished = true
        });

        var updated = new BlogPost
        {
            Id = created.Id,
            Title = "Published post",
            Slug = created.Slug,
            Excerpt = created.Excerpt,
            Body = created.Body,
            IsPublished = false,
            PublishedAt = created.PublishedAt
        };

        var result = await service.UpdateAsync(created.Id, updated);

        Assert.True(result);
        var stored = await context.BlogPosts
            .AsNoTracking()
            .SingleAsync(x => x.Id == created.Id);

        Assert.False(stored.IsPublished);
        Assert.Null(stored.PublishedAt);
    }

    [Fact]
    public async Task GenerateUniqueSlugAsync_GeneratesUniqueSlug()
    {
        var context = CreateContext();
        var service = new BlogPostService(context);

        var existing = new BlogPost
        {
            Title = "My post",
            Slug = "my-post",
            Excerpt = "Excerpt",
            Body = "Body",
            IsPublished = true,
            PublishedAt = DateTime.UtcNow
        };

        await service.CreateAsync(existing);

        var slug = await service.GenerateUniqueSlugAsync("My post");

        Assert.Equal("my-post-2", slug);
    }
}
