using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Client;
using Client.Animals;

namespace Web.Animals;

public partial class Dashboard
{
    private readonly IShelteredClient _shelteredClient;

    public Dashboard(IShelteredClient shelteredClient)
    {
        _shelteredClient = shelteredClient;
    }

    ~Dashboard()
    {
        _shelteredClient.Dispose();
    }

    public string IdText { get; set; } = string.Empty;

    public Guid Id => Guid.TryParse(IdText, out var id) ? id : Guid.Empty;

    public AnimalModel? Animal { get; private set; } = null;

    public async Task GetAnimalAsync()
    {
        try
        {
            Animal = await _shelteredClient.GetAnimalByIdAsync(Id, CancellationToken.None);
        }
        catch (HttpRequestException e)
        {
            _ = e.Message;
            Animal = await Task.FromResult<AnimalModel?>(null);
        }
    }
}
