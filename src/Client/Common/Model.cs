using System;
using System.Text.Json.Serialization;

namespace Client.Common;

/// <summary>
/// Represents a base class for all models.
/// </summary>
public abstract record class Model
{
    /// <summary>
    /// Gets or inits the id of the model.
    /// </summary>
    /// <value>The id of the model.</value>
    [JsonPropertyName("id")]
    public Guid Id { get; init; } = Guid.Empty;
}
