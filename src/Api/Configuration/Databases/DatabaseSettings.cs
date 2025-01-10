using System.Diagnostics.CodeAnalysis;

namespace Api.Configuration.Databases;

[ExcludeFromCodeCoverage]
public abstract record class DatabaseSettings
{
    public required DatabaseProvider Provider { get; init; }

    public required string Database { get; init; }

    public string? Host { get; init; }

    public string? Username { get; init; }

    public string? Password { get; init; }

    public int? Port { get; init; }
}
