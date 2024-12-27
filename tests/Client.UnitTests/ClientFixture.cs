using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Client.Animals;
using Core.Animals;

namespace Client.UnitTests;

[TestFixture]
[TestFixtureSource(nameof(Source))]
internal sealed class ClientFixture(ShelteredClient shelteredClient)
{
    private static IEnumerable<TestFixtureData> Source()
    {
        var httpClient = new HttpClient();
        var options = new FakeOptions();
        yield return new TestFixtureData(new ShelteredClient(httpClient, options));
    }

    ~ClientFixture()
    {
        shelteredClient.Dispose();
    }

    private Guid Id { get; set; }

    [Test]
    [Order(1)]
    public async Task Should_create()
    {
        var animalModel = new AnimalModel { Name = "Lucy", Kind = AnimalKind.Cat };
        var (_, id) = await shelteredClient.CreateAnimalAsync(animalModel);
        Id = id;
        Assert.That(id, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    [Order(2)]
    public async Task Should_exist()
    {
        var exists = await shelteredClient.AnimalExistsByIdAsync(Id);
        Assert.That(exists, Is.True);
    }
}
