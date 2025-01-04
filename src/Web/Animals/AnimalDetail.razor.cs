using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Client;
using Client.Animals;

namespace Web.Animals;

/// <summary>
/// Represents a detailed view of an animal.
/// </summary>
public sealed partial class AnimalDetail(IShelteredClient shelteredClient)
{
    /// <summary>
    /// Finalizes the animal detail and disposes the <see cref="IShelteredClient"/>.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Testing a finalizer is likely difficult and flaky.")]
    ~AnimalDetail()
    {
        shelteredClient.Dispose();
    }

    /// <summary>
    /// Gets or inits the id.
    /// </summary>
    /// <value>The id of the animal.</value>
    [Parameter]
    public required Guid Id { get; init; }

    /// <summary>
    /// Gets or sets the animal.
    /// </summary>
    /// <value>The animal. May be null.</value>
    public AnimalModel? Animal { get; private set; }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        try
        {
            Animal = await shelteredClient.GetAnimalByIdAsync(Id);
        }
        catch
        {
            Animal = null;
        }
    }
}
