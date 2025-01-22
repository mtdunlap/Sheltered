using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Client.Animals;
using Web.Common;

namespace Web.Animals;

/// <summary>
/// A component for uploading an image to the sheltered api.
/// </summary>
/// <param name="handledShelteredClient">
/// A handled client for sending and retrieving data to and from the sheltered api.
/// </param>
public sealed partial class ImageUpload(IHandledShelteredClient handledShelteredClient) : IDisposable
{
    /// <summary>
    /// Finalizes the <see cref="ImageUpload"/> component, disposing the <see cref="IHandledShelteredClient"/>.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Testing a finalizer is likely difficult and flaky.")]
    ~ImageUpload()
    {
        handledShelteredClient.Dispose();
    }

    /// <summary>
    /// Gets or inits the id of the animal.
    /// </summary>
    /// <value>The id of the animal.</value>
    [Parameter]
    [EditorRequired]
    public Guid AnimalId { get; init; }

    /// <summary>
    /// The uploaded file. May be null.
    /// </summary>
    private IBrowserFile? _file = null;

    /// <summary>
    /// Determines if a file has been uploaded.
    /// </summary>
    /// <value>true if a file has been uploaded; otherwise false.</value>
    [MemberNotNullWhen(true, nameof(_file))]
    private bool HasFile => _file is not null;

    private string UploadResultMessage { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a bool indicating if the upload has a result message to show.
    /// </summary>
    /// <value>true if the upload has a result message to show; otherwise false.</value>
    private bool HasUploadResultMessage { get; set; } = false;

    /// <summary>
    /// The maximum allowed size of an uploaded file in bytes.
    /// </summary>
    /// <value>Twenty MebiBytes (20 MiB)</value>
    private const uint MaxFileSizeInBytes = 20_971_520;

    /// <summary>
    /// Sets the uploaded file.
    /// </summary>
    /// <param name="inputFileChangeEventArgs">An event including the uploaded file.</param>
    private void SetFile(InputFileChangeEventArgs inputFileChangeEventArgs)
    {
        _file = inputFileChangeEventArgs.File;
    }

    private async Task UploadFileAsync()
    {
        if (HasFile)
        {
            HasUploadResultMessage = false;
            UploadResultMessage = string.Empty;
            await handledShelteredClient.TryAddImageAsync(AnimalId, _file, MaxFileSizeInBytes, OnSuccess, OnError);
        }
    }

    /// <summary>
    /// Clears the uploaded file and sets a message indicating the upload was successful.
    /// </summary>
    /// <param name="animalImageModel">The created <see cref="AnimalImageModel"/> from the upload.</param>
    private void OnSuccess(AnimalImageModel animalImageModel)
    {
        _file = null;
        HasUploadResultMessage = true;
        UploadResultMessage = "The image was uploaded successfully!";
    }

    /// <summary>
    /// Sets a message indicating the upload was failed based on the type of exception encountered.
    /// </summary>
    /// <param name="exception">The caught <see cref="Exception"/>.</param>
    private void OnError(Exception exception)
    {
        HasUploadResultMessage = true;
        if (exception is HttpRequestException { StatusCode: HttpStatusCode.NotFound })
        {
            UploadResultMessage = "The upload failed because the associated animal was not found.";
        }
        else if (exception is IOException)
        {
            UploadResultMessage = "The selected file is too large.";
        }
        else
        {
            UploadResultMessage = "An unknown error occurred, please try again momentarily.";
        }
    }

    /// <summary>
    /// Disposes the <see cref="IHandledShelteredClient"/>.
    /// </summary>
    public void Dispose()
    {
        handledShelteredClient.Dispose();
        GC.SuppressFinalize(this);
    }
}
