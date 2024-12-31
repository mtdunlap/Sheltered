using Microsoft.AspNetCore.Components;
using Client.Animals;

namespace Web.Animals;

/// <summary>
/// Represents a preview of the animal.
/// </summary>
public sealed partial class AnimalPreview
{
    /// <summary>
    /// Gets or inits the <see cref="AnimalModel"/>.
    /// </summary>
    /// <value>The animal.</value>
    [Parameter]
    [EditorRequired]
    public required AnimalModel Animal { get; init; }
}
