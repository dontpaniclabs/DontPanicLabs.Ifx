namespace DontPanicLabs.Ifx.Mapping.DtoMapper.IntegrationTests;

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
        // SQL Server's physical filename limit rejects long database names.
        // Use a stable short hash when the name exceeds 64 chars.
        if (databaseName.Length > 64)
        {
            var hash = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(databaseName));
            databaseName = "T_" + Convert.ToHexString(hash)[..20];
        }

        var baseConnection = Environment.GetEnvironmentVariable("DTOMAPPER_SQL_CONNECTION");
        if (!string.IsNullOrWhiteSpace(baseConnection))
            return $"{baseConnection};Database={databaseName}";

        return $@"Data Source=(localdb)\mssqllocaldb;Integrated Security=True;MultipleActiveResultSets=True;Database={databaseName};Connection Timeout=300";
    }
}