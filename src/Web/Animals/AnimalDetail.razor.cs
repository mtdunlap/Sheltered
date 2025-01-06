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
/// <param name="shelteredClient">An <see cref="IShelteredClient"/> to request data from the sheltered api.</param>
/// <param name="navigationManager">A <see cref="NavigationManager"/> for navigating to other pages.</param>
public sealed partial class AnimalDetail(IShelteredClient shelteredClient, NavigationManager navigationManager)
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

    /// <summary>
    /// Navigates to the update animal page.
    /// </summary>
    public void NavigateToUpdateAnimalPage()
    {
        navigationManager.NavigateTo($"animals/update/{Id}");
    }

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
