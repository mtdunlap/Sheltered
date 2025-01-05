using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Client;
using Client.Animals;
using Core.Animals;

namespace Web.Animals;

/// <summary>
/// A view for adding a new animal.
/// </summary>
/// <param name="shelteredClient">An <see cref="IShelteredClient"/> to request data from the sheltered api.</param>
/// <param name="navigationManager">A <see cref="NavigationManager"/> for navigating to other pages.</param>
public sealed partial class NewAnimal(IShelteredClient shelteredClient, NavigationManager navigationManager)
{
    /// <summary>
    /// Finalizes the dashboard and disposes the <see cref="IShelteredClient"/>.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Testing a finalizer is likely difficult and flaky.")]
    ~NewAnimal()
    {
        shelteredClient.Dispose();
    }

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
    /// Submits a new <see cref="AnimalModel"/> with the current name and kind.
    /// </summary>
    /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task CreateNewAnimalAsync()
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
            var (_, id) = await shelteredClient.CreateAnimalAsync(animalModel);
            navigationManager.NavigateTo($"/animals/{id}");
        }
        catch
        {
            HasSubmissionErrors = true;
            ErrorMessage = "An error occurred, please try again momentarily.";
        }
    }
}
