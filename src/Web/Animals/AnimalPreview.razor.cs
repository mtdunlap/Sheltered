using System.Linq;
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

    /// <summary>
    /// Gets or inits the height of the image.
    /// </summary>
    /// <value>The height of the image in pixels.</value>
    [Parameter]
    [EditorRequired]
    public required uint Height { get; init; }

    /// <summary>
    /// Gets or inits the width of the image.
    /// </summary>
    /// <value>The width of the image in pixels.</value>
    [Parameter]
    [EditorRequired]
    public required uint Width { get; init; }

    /// <summary>
    /// Gets the location of the first image of the animal.
    /// </summary>
    /// <value>The location of the first image of the animal.</value>
    private string Location => Animal.Images.First().Location;
}
