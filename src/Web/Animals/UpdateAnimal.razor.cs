using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Client;
using Client.Animals;
using Core.Animals;

namespace Web.Animals;

/// <summary>
/// A view for updating an existing animal.
/// </summary>
/// <param name="shelteredClient">An <see cref="IShelteredClient"/> to request data from the sheltered api.</param>
/// <param name="navigationManager">A <see cref="NavigationManager"/> for navigating to other pages.</param>
public sealed partial class UpdateAnimal(IShelteredClient shelteredClient, NavigationManager navigationManager)
{
    /// <summary>
    /// Finalizes the dashboard and disposes the <see cref="IShelteredClient"/>.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Testing a finalizer is likely difficult and flaky.")]
    ~UpdateAnimal()
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
    /// Gets or sets the current animal.
    /// </summary>
    /// <value>The current animal. May be null.</value>
    public AnimalModel? Current { get; private set; } = null;

    /// <summary>
    /// Gets or sets the animals name.
    /// </summary>
    /// <value>The name of the animal.</value>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the kind of the animal.
    /// </summary>
    /// <value>The kind of the animal.</value>
    public AnimalKind Kind { get; set; } = AnimalKind.Unspecified;

    /// <summary>
    /// Gets or sets a <see cref="bool"/> indicating if the submission has any errors.
    /// </summary>
    /// <value>true if the submission has errors; otherwise false.</value>
    public bool HasSubmissionErrors { get; set; } = false;

    /// <summary>
    /// Gets or sets a <see cref="string"/> representing the submission error message.
    /// </summary>
    /// <value>The last erorr message from a failed submission.</value>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// Updates the current <see cref="AnimalModel"/> with the updated information.
    /// </summary>
    /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task UpdateAnimalAsync()
    {
        try
        {
            HasSubmissionErrors = false;
            ErrorMessage = string.Empty;
            var animalModel = new AnimalModel
            {
                Name = Name,
                Kind = Kind
            };
            var isSuccessful = await shelteredClient.UpdateAnimalByIdAsync(Id, animalModel);
            if (isSuccessful)
            {
                navigationManager.NavigateTo($"/animals/{Id}");
            }
            else
            {
                HasSubmissionErrors = true;
                ErrorMessage = "The animal could not be updated as it does not exist.";
            }
        }
        catch
        {
            HasSubmissionErrors = true;
            ErrorMessage = "An unknown error occurred, please try again momentarily.";
        }
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        try
        {
            Current = await shelteredClient.GetAnimalByIdAsync(Id);
        }
        catch
        {
            Current = null;
        }
    }
}
