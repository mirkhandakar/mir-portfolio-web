using Microsoft.EntityFrameworkCore;
using MirPortfolio.Web.Data;
using MirPortfolio.Web.Models;
using MirPortfolio.Web.Services;

namespace MirPortfolio.Web.Tests;

public class ContactMessageServiceTests : ServiceTestBase
{
    [Fact]
    public async Task CreateAsync_PersistsMessageWithDefaults()
    {
        var context = CreateContext();
        var service = new ContactMessageService(context);
        var message = new ContactMessage
        {
            Name = "Test User",
            Email = "test@example.com",
            Subject = "Test subject",
            Body = "Test body"
        };

        var created = await service.CreateAsync(message);

        var stored = await context.ContactMessages
            .AsNoTracking()
            .SingleAsync();

        Assert.Equal("Test User", stored.Name);
        Assert.Equal("test@example.com", stored.Email);
        Assert.Equal("Test subject", stored.Subject);
        Assert.Equal("Test body", stored.Body);
        Assert.Equal(DateTime.UtcNow.Date, created.CreatedAt.Date);
    }

    [Fact]
    public async Task GetUnreadCountAsync_CountsOnlyUnreadMessages()
    {
        var context = CreateContext();
        var service = new ContactMessageService(context);

        await service.CreateAsync(new ContactMessage
        {
            Name = "User One",
            Email = "nothing",
            Subject = "Subject"
            , Body = "Body"
            , IsRead = true
        });

        await service.CreateAsync(new ContactMessage
        {
            Name = "User Two",
            Email = "test",
            Subject = "Subject",
            Body = "Body"
        });

        Assert.Equal(1, await service.GetUnreadCountAsync());
    }

    [Fact]
    public async Task MarkReadAsync_TogglesReadState()
    {
        var context = CreateContext();
        var service = new ContactMessageService(context);

        var created = await service.CreateAsync(new ContactMessage
        {
            Name = "Test User",
            Email = "test@example.com",
            Subject = "Subject",
            Body = "Body"
        });

        var read = await service.MarkReadAsync(created.Id, true);
        var unread = await service.MarkReadAsync(created.Id, false);

        Assert.True(read);
        Assert.True(unread);
    }

    [Fact]
    public async Task DeleteAsync_RemovesMessage()
    {
        var context = CreateContext();
        var service = new ContactMessageService(context);

        var created = await service.CreateAsync(new ContactMessage
        {
            Name = "Test User",
            Email = "test@example.com",
            Subject = "Subject",
            Body = "Body"
        });

        var deleted = await service.DeleteAsync(created.Id);

        Assert.True(deleted);

        Assert.Empty(context.ContactMessages);
    }
}
