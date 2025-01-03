using System;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Client;
using Core.Animals;
using Data;
using Data.Animals;

namespace Api.Benchmarks.Animals;

public class AnimalControllerBenchmarks
{
    private IIntegrationTestDatabaseConfigurer _databaseConfigurer = null!;
    private IntegrationTestWebApplicationFactory<Api.Program> _webApplicationFactory = null!;
    private ShelteredClient _shelteredClient = null!;
    private CancellationTokenSource _cancellationTokenSource = null!;
    private Guid _id = default;

    [GlobalSetup]
    public void OnceBeforeAllBenchmarks()
    {
        _databaseConfigurer = SqliteIntegrationTestDatabaseConfigurer<ShelteredContext>.RandomlyNamedInMemoryConfigurer;
        _webApplicationFactory = new(_databaseConfigurer);
        _shelteredClient = _webApplicationFactory.CreateShelteredClient();
        _cancellationTokenSource = new();
    }

    [IterationSetup(Target = nameof(AnimalExistsByIdAsync__When_no_animal_exists))]
    public void Before_AnimalExistsByIdAsync__When_no_animal_exists()
    {
        _id = Guid.NewGuid();
    }

    [Benchmark]
    public async Task AnimalExistsByIdAsync__When_no_animal_exists()
    {
        _ = await _shelteredClient.AnimalExistsByIdAsync(_id, _cancellationTokenSource.Token);
    }

    [IterationCleanup(Target = nameof(AnimalExistsByIdAsync__When_no_animal_exists))]
    public void After_AnimalExistsByIdAsync__When_no_animal_exists()
    {
        _id = default;
    }

    [IterationSetup(Target = nameof(AnimalExistsByIdAsync__When_the_animal_does_exist))]
    public void Before_AnimalExistsByIdAsync__When_the_animal_does_exist()
    {
        var animalEntity = new AnimalEntity
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat
        };
        using var shelteredContext = _webApplicationFactory.GetDbContext<ShelteredContext>();
        _ = shelteredContext.Add(animalEntity);
        _ = shelteredContext.SaveChanges();
        _id = animalEntity.Id;
    }

    [Benchmark]
    public async Task AnimalExistsByIdAsync__When_the_animal_does_exist()
    {
        _ = await _shelteredClient.AnimalExistsByIdAsync(_id, _cancellationTokenSource.Token);
    }

    [IterationCleanup(Target = nameof(AnimalExistsByIdAsync__When_the_animal_does_exist))]
    public void After_AnimalExistsByIdAsync__When_the_animal_does_exist()
    {
        using var shelteredContext = _webApplicationFactory.GetDbContext<ShelteredContext>();
        var animalEntity = shelteredContext.Find<AnimalEntity>(_id);
        _ = shelteredContext.Remove(animalEntity!);
        _ = shelteredContext.SaveChanges();
        _id = default;
    }

    [GlobalCleanup]
    public void OnceAfterAllBenchmarks()
    {
        _cancellationTokenSource.Dispose();
        _shelteredClient.Dispose();
        _webApplicationFactory.Dispose();
    }
}
