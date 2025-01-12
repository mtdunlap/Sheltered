using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;

namespace Tests.Common.Containers.Api;

public sealed class ApiImage : IImage
{
    public const ushort HttpPort = 80;

    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

    private readonly DockerImage _image = new("localhost/testcontainers", "api", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());

    public async Task StartAsync()
    {
        await _semaphoreSlim.WaitAsync().ConfigureAwait(false);

        try
        {
            await new ImageFromDockerfileBuilder()
                .WithName(this)
                .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), string.Empty)
                .WithDockerfile("tests/Tests.Common/Containers/Api/Dockerfile")
                .WithBuildArgument("RESOURCE_REAPER_SESSION_ID", ResourceReaper.DefaultSessionId.ToString("D")) // https://github.com/testcontainers/testcontainers-dotnet/issues/602.
                .WithDeleteIfExists(false)
                .Build()
                .CreateAsync()
                .ConfigureAwait(false);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public async Task DisposeAsync()
    {
        await Task.CompletedTask;
    }

    public string Repository => _image.Repository;

    public string Tag => _image.Tag;

    public string FullName => _image.FullName;

    public string Registry => throw new NotImplementedException();

    public string Digest => throw new NotImplementedException();

    public string GetHostname() => _image.GetHostname();

    public bool MatchLatestOrNightly() => throw new NotImplementedException();
    public bool MatchVersion(Predicate<string> predicate) => throw new NotImplementedException();
    public bool MatchVersion(Predicate<Version> predicate) => throw new NotImplementedException();
}
