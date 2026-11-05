using Microsoft.EntityFrameworkCore;
using MirPortfolio.Web.Data;

namespace MirPortfolio.Web.Tests;

public abstract class ServiceTestBase
{
    protected static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}
