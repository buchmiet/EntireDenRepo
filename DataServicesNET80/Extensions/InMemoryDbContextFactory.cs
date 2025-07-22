using Microsoft.EntityFrameworkCore;

namespace DataServicesNET80.Extensions;

public class InMemoryDbContextFactory 
{
    private readonly DbContextOptions<Time4PartsContext> _options;

    public InMemoryDbContextFactory(string dbName)
    {
        _options = new DbContextOptionsBuilder<Time4PartsContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
    }

    public Time4PartsContext GetWriteableContext()
    {
        return new Time4PartsContext(_options);
    }
}