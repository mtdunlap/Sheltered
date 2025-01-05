using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Client;
using Client.Animals;

namespace Web.Animals;

/// <summary>
/// A dashboard view of all the animals.
/// </summary>
/// <param name="shelteredClient">An <see cref="IShelteredClient"/> to request data from the sheltered api.</param>
/// <param name="navigationManager">A <see cref="NavigationManager"/> for navigating to other pages.</param>
public sealed partial class Dashboard(IShelteredClient shelteredClient, NavigationManager navigationManager)
{
    /// <summary>
    /// Finalizes the dashboard and disposes the <see cref="IShelteredClient"/>.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Testing a finalizer is likely difficult and flaky.")]
    ~Dashboard()
    {
        shelteredClient.Dispose();
    }

    /// <summary>
    /// Gets or sets the <see cref="IReadOnlyDictionary{TKey, TValue}"/> of <see cref="AnimalModel"/>s.
    /// </summary>
    /// <value>The animals to display on the dashboard.</value>
    public IReadOnlyDictionary<Guid, AnimalModel> IdsToAnimals { get; private set; } = new Dictionary<Guid, AnimalModel>();

    /// <summary>
    /// Navigates to the create animal page.
    /// </summary>
    public void NavigateToCreateAnimalPage()
    {
        navigationManager.NavigateTo("animals/new");
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        try
        {
            IdsToAnimals = await shelteredClient.ListAnimalsAsync();
        }
        catch
        {
            IdsToAnimals = new Dictionary<Guid, AnimalModel>();
        }
    }
}
