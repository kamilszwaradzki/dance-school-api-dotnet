using DanceSchool.Api.Data;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace DanceSchool.Tests.TestHelpers;

public class SqlServerFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-CU28-ubuntu-22.04").Build();

    public Task InitializeAsync() => _container.StartAsync();

    public Task DisposeAsync() => _container.DisposeAsync().AsTask();

    public async Task<(string ConnectionString, DanceSchoolDbContext Context)> CreateDatabaseAsync(
        TimeProvider? time = null)
    {
        var cs = new SqlConnectionStringBuilder(_container.GetConnectionString())
        {
            InitialCatalog = $"test_{Guid.NewGuid():N}"
        }.ConnectionString;

        var db = OpenContext(cs, time);
        await db.Database.MigrateAsync();
        return (cs, db);
    }

    public DanceSchoolDbContext OpenContext(string connectionString, TimeProvider? time = null)
    {
        var options = new DbContextOptionsBuilder<DanceSchoolDbContext>()
            .UseSqlServer(connectionString)
            .Options;
        return new DanceSchoolDbContext(options, time ?? TimeProvider.System);
    }
}