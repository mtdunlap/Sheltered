namespace Api.Configuration.Databases;

/// <summary>
/// Represents the database configuration settings.
/// </summary>
public sealed record class DatabaseSettings
{
    /// <summary>
    /// The key for the database settings.
    /// </summary>
    public const string Key = "Database";

    /// <summary>
    /// Gets or inits the data source the database connection string.
    /// </summary>
    /// <value>The data source.</value>
    public required string DataSource { get; init; }
}
