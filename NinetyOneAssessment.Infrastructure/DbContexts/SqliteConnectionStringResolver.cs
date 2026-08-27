using Microsoft.Data.Sqlite;

namespace NinetyOneAssessment.Infrastructure.DbContexts;

internal static class SqliteConnectionStringResolver
{
    public const string ConnectionStringName = "ScoresDatabase";
    public const string DefaultConnectionString = "Data Source=scores.db";

    // A relative Data Source is rebased onto the solution root so that every entry point -
    // the console app and the API - reads and writes the same database file, whichever
    // directory the process was launched from and whichever bin folder it was built into.
    // An absolute Data Source is left untouched, so a deployment can override the location.
    public static string Resolve(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                $"Connection string '{ConnectionStringName}' is not configured. " +
                "Ensure appsettings.json is present alongside the executable.");

        var builder = new SqliteConnectionStringBuilder(connectionString);

        if (string.IsNullOrWhiteSpace(builder.DataSource))
            throw new InvalidOperationException(
                $"Connection string '{ConnectionStringName}' does not specify a Data Source.");

        if (builder.Mode != SqliteOpenMode.Memory
            && !builder.DataSource.Equals(":memory:", StringComparison.OrdinalIgnoreCase)
            && !Path.IsPathRooted(builder.DataSource))
        {
            builder.DataSource = Path.Combine(SharedDataDirectory(), builder.DataSource);
        }

        return builder.ToString();
    }

    // Walks up from the executable looking for the solution file, so the database sits at the
    // repository root where it can be opened from the IDE. A published build has no solution
    // above it, in which case the executable's own directory is the sensible home.
    public static string SharedDataDirectory()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (directory.EnumerateFiles("*.sln").Any() || directory.EnumerateFiles("*.slnx").Any())
                return directory.FullName;

            directory = directory.Parent;
        }

        return AppContext.BaseDirectory;
    }
}
