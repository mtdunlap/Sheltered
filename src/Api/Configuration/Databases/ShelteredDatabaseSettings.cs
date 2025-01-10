using System.Diagnostics.CodeAnalysis;

namespace Api.Configuration.Databases;

[ExcludeFromCodeCoverage]
public sealed record class ShelteredDatabaseSettings : DatabaseSettings, IConfigurationSettings
{
    public static string SectionKey { get; } = "ShelteredDatabase";

    public static string ErrorMessage { get; } = "The configuration settings ";
}
