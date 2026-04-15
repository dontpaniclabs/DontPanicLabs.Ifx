namespace AutoMapper.IntegrationTests;

public abstract class IntegrationTest<TInitializer> : AutoMapperSpecBase, IAsyncLifetime where TInitializer : IInitializer, new()
{
    Task IAsyncLifetime.DisposeAsync() => Task.CompletedTask;
    Task IAsyncLifetime.InitializeAsync() => new TInitializer().Migrate();
}
public interface IInitializer
{
    Task Migrate();
}
public class DropCreateDatabaseAlways<TContext> : IInitializer where TContext : DbContext, new()
{
    protected virtual void Seed(TContext context){}
    public async Task Migrate()
    {
        await using var context = new TContext();
        var database = context.Database;
        await database.EnsureDeletedAsync();
        var strategy = database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () => await database.EnsureCreatedAsync());

        Seed(context);

        await context.SaveChangesAsync();
    }
}
public abstract class LocalDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlServer(
        BuildConnectionString(GetType().ToString()),
        o => o.EnableRetryOnFailure(maxRetryCount: 10).CommandTimeout(120));

    private static string BuildConnectionString(string databaseName)
    {
        var baseConnection = Environment.GetEnvironmentVariable("DTOMAPPER_SQL_CONNECTION");
        if (!string.IsNullOrWhiteSpace(baseConnection))
            return $"{baseConnection};Database={databaseName}";

        return $@"Data Source=(localdb)\mssqllocaldb;Integrated Security=True;MultipleActiveResultSets=True;Database={databaseName};Connection Timeout=300";
    }
}