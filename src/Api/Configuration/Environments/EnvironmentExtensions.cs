using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Hosting;

namespace Api.Configuration.Environments;

/// <summary>
/// Extensions for determining application environment.
/// </summary>
[ExcludeFromCodeCoverage]
public static class EnvironmentExtensions
{
    /// <summary>
    /// Determines if the host environment is local.
    /// </summary>
    /// <param name="hostEnvironment">The <see cref="IHostEnvironment"/>.</param>
    /// <returns>true if the environment is local; otherwise false.</returns>
    public static bool IsLocal(this IHostEnvironment hostEnvironment)
    {
        return string.Equals(hostEnvironment.EnvironmentName, "local", StringComparison.InvariantCultureIgnoreCase);
    }
}
