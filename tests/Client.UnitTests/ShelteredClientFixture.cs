using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using RichardSzalay.MockHttp;
using Client.Animals;
using Core.Animals;

namespace Client.UnitTests;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public sealed class ShelteredClientFixture
{
    private static readonly CancellationTokenSource CancellationTokenSource = new();

    ~ShelteredClientFixture()
    {
        CancellationTokenSource.Dispose();
    }

    private static IEnumerable<TestCaseData> AnimalExistsAsync__InvalidResponseStatusCodeSource()
    {
        yield return new TestCaseData(HttpStatusCode.OK);
        yield return new TestCaseData(HttpStatusCode.BadRequest);
        yield return new TestCaseData(HttpStatusCode.InternalServerError);
    }

    [Test]
    [TestCaseSource(nameof(AnimalExistsAsync__InvalidResponseStatusCodeSource))]
    public void AnimalExistsAsync__Should_throw_an_http_request_exception_When_response_status_code_is_invalid(HttpStatusCode statusCode)
    {
        const string baseUrl = "http://localhost:5108";
        var id = Guid.NewGuid();
        using var handler = new MockHttpMessageHandler();
        var request = handler
            .Expect($"{baseUrl}/animal/{id}")
            .Respond(statusCode);
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl, UriKind.Absolute)
        };
        using var shelteredClient = new ShelteredClient(httpClient);

        Assert.Multiple(() =>
        {
            const string expectedMessage = "statusCode is neither NoContent nor NotFound.";
            Assert.That(async () =>
            {
                _ = await shelteredClient.AnimalExistsByIdAsync(id, CancellationTokenSource.Token);
            }, Throws.TypeOf<HttpRequestException>()
                .With.Message.EqualTo(expectedMessage)
                .And.InnerException.Null
                .And.Property(nameof(HttpRequestException.StatusCode)).EqualTo(statusCode));
            Assert.That(handler.GetMatchCount(request), Is.EqualTo(1));
            Assert.That(() => handler.VerifyNoOutstandingExpectation(), Throws.Nothing);
        });
    }

    private static IEnumerable<TestCaseData> AnimalExistsAsync__ValidResponseStatusCodeSource()
    {
        yield return new TestCaseData(HttpStatusCode.NoContent);
        yield return new TestCaseData(HttpStatusCode.NotFound);
    }

    [Test]
    [TestCaseSource(nameof(AnimalExistsAsync__ValidResponseStatusCodeSource))]
    public void AnimalExistsAsync__Should_not_throw_an_exception_When_response_status_code_is_valid(HttpStatusCode statusCode)
    {
        const string baseUrl = "http://localhost:5108";
        var id = Guid.NewGuid();
        using var handler = new MockHttpMessageHandler();
        var request = handler
            .Expect($"{baseUrl}/animal/{id}")
            .Respond(statusCode);
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl, UriKind.Absolute)
        };
        using var shelteredClient = new ShelteredClient(httpClient);

        Assert.Multiple(() =>
        {
            Assert.That(async () =>
            {
                _ = await shelteredClient.AnimalExistsByIdAsync(id, CancellationTokenSource.Token);
            }, Throws.Nothing);
            Assert.That(handler.GetMatchCount(request), Is.EqualTo(1));
            Assert.That(() => handler.VerifyNoOutstandingExpectation(), Throws.Nothing);
        });
    }

    [Test]
    public async Task AnimalExistsAsync__Should_return_true_When_the_response_status_code_is_204_no_content()
    {
        const string baseUrl = "http://localhost:5108";
        var id = Guid.NewGuid();
        using var handler = new MockHttpMessageHandler();
        var request = handler
            .Expect($"{baseUrl}/animal/{id}")
            .Respond(HttpStatusCode.NoContent);
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl, UriKind.Absolute)
        };
        using var shelteredClient = new ShelteredClient(httpClient);

        var actual = await shelteredClient.AnimalExistsByIdAsync(id, CancellationTokenSource.Token);

        Assert.Multiple(() =>
        {
            Assert.That(actual, Is.True);
            Assert.That(handler.GetMatchCount(request), Is.EqualTo(1));
            Assert.That(() => handler.VerifyNoOutstandingExpectation(), Throws.Nothing);
        });
    }

    [Test]
    public async Task AnimalExistsAsync__Should_return_false_When_the_response_status_code_is_404_not_found()
    {
        const string baseUrl = "http://localhost:5108";
        var id = Guid.NewGuid();
        using var handler = new MockHttpMessageHandler();
        var request = handler
            .Expect($"{baseUrl}/animal/{id}")
            .Respond(HttpStatusCode.NotFound);
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl, UriKind.Absolute)
        };
        using var shelteredClient = new ShelteredClient(httpClient);

        var actual = await shelteredClient.AnimalExistsByIdAsync(id, CancellationTokenSource.Token);

        Assert.Multiple(() =>
        {
            Assert.That(actual, Is.False);
            Assert.That(handler.GetMatchCount(request), Is.EqualTo(1));
            Assert.That(() => handler.VerifyNoOutstandingExpectation(), Throws.Nothing);
        });
    }

    private static IEnumerable<TestCaseData> CreateAnimalAsync_InvalidResponseStatusCodeSource()
    {
        yield return new TestCaseData(HttpStatusCode.OK);
        yield return new TestCaseData(HttpStatusCode.NotFound);
        yield return new TestCaseData(HttpStatusCode.InternalServerError);
    }

    [Test]
    [TestCaseSource(nameof(CreateAnimalAsync_InvalidResponseStatusCodeSource))]
    public void CreateAnimalAsync__Should_throw_an_http_request_exception_When_response_status_code_is_not_201_created(HttpStatusCode statusCode)
    {
        const string baseUrl = "http://localhost:5108";
        var animalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat,
            Sex = AnimalSex.Female
        };
        using var handler = new MockHttpMessageHandler();
        var request = handler
            .Expect($"{baseUrl}/animal")
                .WithJsonContent(animalModel)
            .Respond(statusCode);
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl, UriKind.Absolute)
        };
        using var shelteredClient = new ShelteredClient(httpClient);

        Assert.Multiple(() =>
        {
            const string expectedMessage = "statusCode is not Created.";
            Assert.That(async () =>
            {
                _ = await shelteredClient.CreateAnimalAsync(animalModel, CancellationTokenSource.Token);
            }, Throws.TypeOf<HttpRequestException>().With.Message.EqualTo(expectedMessage));
            Assert.That(handler.GetMatchCount(request), Is.EqualTo(1));
            Assert.That(() => handler.VerifyNoOutstandingExpectation(), Throws.Nothing);
        });
    }

    [Test]
    public void CreateAnimalAsync__Should_throw_an_http_request_exception_When_the_response_does_not_include_a_location_header()
    {
        const string baseUrl = "http://localhost:5108";
        var animalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat,
            Sex = AnimalSex.Female
        };
        using var handler = new MockHttpMessageHandler();
        var request = handler
            .Expect($"{baseUrl}/animal")
                .WithJsonContent(animalModel)
            .Respond(HttpStatusCode.Created, JsonContent.Create(animalModel));
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl, UriKind.Absolute)
        };
        using var shelteredClient = new ShelteredClient(httpClient);

        Assert.Multiple(() =>
        {
            const string expectedMessage = "The response did not include a location header.";
            Assert.That(async () =>
            {
                _ = await shelteredClient.CreateAnimalAsync(animalModel, CancellationTokenSource.Token);
            }, Throws.TypeOf<HttpRequestException>().With.Message.EqualTo(expectedMessage));
            Assert.That(handler.GetMatchCount(request), Is.EqualTo(1));
            Assert.That(() => handler.VerifyNoOutstandingExpectation(), Throws.Nothing);
        });
    }

    [Test]
    public void CreateAnimalAsync__Should_throw_an_http_request_exception_When_the_location_headers_last_segment_was_not_an_id()
    {
        const string baseUrl = "http://localhost:5108";
        var animalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat,
            Sex = AnimalSex.Female
        };
        using var handler = new MockHttpMessageHandler();
        var request = handler
            .Expect($"{baseUrl}/animal")
                .WithJsonContent(animalModel)
            .Respond(HttpStatusCode.Created,
                new Dictionary<string, string>() { { "Location", $"{baseUrl}/animal/notaguid" } },
                JsonContent.Create(animalModel));
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl, UriKind.Absolute)
        };
        using var shelteredClient = new ShelteredClient(httpClient);

        Assert.Multiple(() =>
        {
            const string expectedMessage = "The location header did not include an id.";
            Assert.That(async () =>
            {
                _ = await shelteredClient.CreateAnimalAsync(animalModel, CancellationTokenSource.Token);
            }, Throws.TypeOf<HttpRequestException>().With.Message.EqualTo(expectedMessage));
            Assert.That(handler.GetMatchCount(request), Is.EqualTo(1));
            Assert.That(() => handler.VerifyNoOutstandingExpectation(), Throws.Nothing);
        });
    }

    [Test]
    public void CreateAnimalAsync__Should_throw_an_http_request_exception_When_the_response_json_animal_model_is_null()
    {
        const string baseUrl = "http://localhost:5108";
        var id = Guid.NewGuid();
        var animalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat,
            Sex = AnimalSex.Female
        };
        using var handler = new MockHttpMessageHandler();
        var request = handler
            .Expect($"{baseUrl}/animal")
                .WithJsonContent(animalModel)
            .Respond(HttpStatusCode.Created,
                new Dictionary<string, string>() { { "Location", $"{baseUrl}/animal/{id}" } },
                JsonContent.Create<AnimalModel?>(null));
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl, UriKind.Absolute)
        };
        using var shelteredClient = new ShelteredClient(httpClient);

        Assert.Multiple(() =>
        {
            const string expectedMessage = "The deserialized json AnimalModel was null.";
            Assert.That(async () =>
            {
                _ = await shelteredClient.CreateAnimalAsync(animalModel, CancellationTokenSource.Token);
            }, Throws.TypeOf<HttpRequestException>().With.Message.EqualTo(expectedMessage));
            Assert.That(handler.GetMatchCount(request), Is.EqualTo(1));
            Assert.That(() => handler.VerifyNoOutstandingExpectation(), Throws.Nothing);
        });
    }

    [Test]
    public async Task CreateAnimalAsync__Should_return_the_created_animal_model_and_its_id_When_the_response_location_header_does_not_end_with_a_forward_slash()
    {
        const string baseUrl = "http://localhost:5108";
        var id = Guid.NewGuid();
        var animalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat,
            Sex = AnimalSex.Female
        };
        using var handler = new MockHttpMessageHandler();
        var request = handler
            .Expect($"{baseUrl}/animal")
                .WithJsonContent(animalModel)
            .Respond(HttpStatusCode.Created,
                new Dictionary<string, string>() { { "Location", $"{baseUrl}/animal/{id}" } },
                JsonContent.Create(animalModel));
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl, UriKind.Absolute)
        };
        using var shelteredClient = new ShelteredClient(httpClient);

        var (created, createdId) = await shelteredClient.CreateAnimalAsync(animalModel, CancellationTokenSource.Token);

        Assert.Multiple(() =>
        {
            Assert.That(created, Is.EqualTo(animalModel));
            Assert.That(createdId, Is.EqualTo(id));
            Assert.That(handler.GetMatchCount(request), Is.EqualTo(1));
            Assert.That(() => handler.VerifyNoOutstandingExpectation(), Throws.Nothing);
        });
    }

    [Test]
    public async Task CreateAnimalAsync__Should_return_the_created_animal_model_and_its_id_When_the_response_location_header_ends_with_a_forward_slash()
    {
        const string baseUrl = "http://localhost:5108";
        var id = Guid.NewGuid();
        var animalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat,
            Sex = AnimalSex.Female
        };
        using var handler = new MockHttpMessageHandler();
        var request = handler
            .Expect($"{baseUrl}/animal")
                .WithJsonContent(animalModel)
            .Respond(HttpStatusCode.Created,
                new Dictionary<string, string>() { { "Location", $"{baseUrl}/animal/{id}/" } },
                JsonContent.Create(animalModel));
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl, UriKind.Absolute)
        };
        using var shelteredClient = new ShelteredClient(httpClient);

        var (created, createdId) = await shelteredClient.CreateAnimalAsync(animalModel, CancellationTokenSource.Token);

        Assert.Multiple(() =>
        {
            Assert.That(created, Is.EqualTo(animalModel));
            Assert.That(createdId, Is.EqualTo(id));
            Assert.That(handler.GetMatchCount(request), Is.EqualTo(1));
            Assert.That(() => handler.VerifyNoOutstandingExpectation(), Throws.Nothing);
        });
    }

    private static IEnumerable<TestCaseData> DeleteAnimalByIdAsync__InvalidResponseStatusCodeSource()
    {
        yield return new TestCaseData(HttpStatusCode.OK);
        yield return new TestCaseData(HttpStatusCode.BadRequest);
        yield return new TestCaseData(HttpStatusCode.InternalServerError);
    }

    [Test]
    [TestCaseSource(nameof(DeleteAnimalByIdAsync__InvalidResponseStatusCodeSource))]
    public void DeleteAnimalByIdAsync__Should_throw_an_http_request_exception_When_the_response_status_code_is_invalid(HttpStatusCode statusCode)
    {
        const string baseUrl = "http://localhost:5108";
        var id = Guid.NewGuid();
        using var handler = new MockHttpMessageHandler();
        var request = handler
            .Expect($"{baseUrl}/animal/{id}")
            .Respond(statusCode);
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl, UriKind.Absolute)
        };
        using var shelteredClient = new ShelteredClient(httpClient);

        Assert.Multiple(() =>
        {
            const string expectedMessage = "statusCode is neither NoContent nor NotFound.";
            Assert.That(async () =>
            {
                _ = await shelteredClient.DeleteAnimalByIdAsync(id, CancellationTokenSource.Token);
            }, Throws.TypeOf<HttpRequestException>().With.Message.EqualTo(expectedMessage));
            Assert.That(handler.GetMatchCount(request), Is.EqualTo(1));
            Assert.That(() => handler.VerifyNoOutstandingExpectation(), Throws.Nothing);
        });
    }

    private static IEnumerable<TestCaseData> DeleteAnimalByIdAsync__ValidResponseStatusCodeSource()
    {
        yield return new TestCaseData(HttpStatusCode.NoContent);
        yield return new TestCaseData(HttpStatusCode.NotFound);
    }

    [Test]
    [TestCaseSource(nameof(DeleteAnimalByIdAsync__ValidResponseStatusCodeSource))]
    public void DeleteAnimalByIdAsync__Should_not_throw_When_the_response_status_code_is_valid(HttpStatusCode statusCode)
    {
        const string baseUrl = "http://localhost:5108";
        var id = Guid.NewGuid();
        using var handler = new MockHttpMessageHandler();
        var request = handler
            .Expect($"{baseUrl}/animal/{id}")
            .Respond(statusCode);
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl, UriKind.Absolute)
        };
        using var shelteredClient = new ShelteredClient(httpClient);

        Assert.Multiple(() =>
        {
            Assert.That(async () =>
            {
                _ = await shelteredClient.DeleteAnimalByIdAsync(id, CancellationTokenSource.Token);
            }, Throws.Nothing);
            Assert.That(handler.GetMatchCount(request), Is.EqualTo(1));
            Assert.That(() => handler.VerifyNoOutstandingExpectation(), Throws.Nothing);
        });
    }

    [Test]
    public async Task DeleteAnimalByIdAsync__Should_return_true_When_the_animal_is_found_and_deleted()
    {
        const string baseUrl = "http://localhost:5108";
        var id = Guid.NewGuid();
        using var handler = new MockHttpMessageHandler();
        var request = handler
            .Expect($"{baseUrl}/animal/{id}")
            .Respond(HttpStatusCode.NoContent);
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl, UriKind.Absolute)
        };
        using var shelteredClient = new ShelteredClient(httpClient);

        var actual = await shelteredClient.DeleteAnimalByIdAsync(id, CancellationTokenSource.Token);

        Assert.Multiple(() =>
        {
            Assert.That(actual, Is.True);
            Assert.That(handler.GetMatchCount(request), Is.EqualTo(1));
            Assert.That(() => handler.VerifyNoOutstandingExpectation(), Throws.Nothing);
        });
    }

    [Test]
    public async Task DeleteAnimalByIdAsync__Should_return_false_When_an_animal_is_not_found()
    {
        const string baseUrl = "http://localhost:5108";
        var id = Guid.NewGuid();
        using var handler = new MockHttpMessageHandler();
        var request = handler
            .Expect($"{baseUrl}/animal/{id}")
            .Respond(HttpStatusCode.NotFound);
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl, UriKind.Absolute)
        };
        using var shelteredClient = new ShelteredClient(httpClient);

        var actual = await shelteredClient.DeleteAnimalByIdAsync(id, CancellationTokenSource.Token);

        Assert.Multiple(() =>
        {
            Assert.That(actual, Is.False);
            Assert.That(handler.GetMatchCount(request), Is.EqualTo(1));
            Assert.That(() => handler.VerifyNoOutstandingExpectation(), Throws.Nothing);
        });
    }

    private static IEnumerable<TestCaseData> GetAnimalByIdAsync__InvalidRessponseStatusCodeSource()
    {
        yield return new TestCaseData(HttpStatusCode.BadRequest);
        yield return new TestCaseData(HttpStatusCode.NotFound);
        yield return new TestCaseData(HttpStatusCode.InternalServerError);
    }

    [Test]
    [TestCaseSource(nameof(GetAnimalByIdAsync__InvalidRessponseStatusCodeSource))]
    public void GetAnimalByIdAsync__Should_throw_an_http_request_exception_When_the_response_status_code_is_invalid(HttpStatusCode statusCode)
    {
        const string baseUrl = "http://localhost:5108";
        var id = Guid.NewGuid();
        using var handler = new MockHttpMessageHandler();
        var request = handler
            .Expect($"{baseUrl}/animal/{id}")
            .Respond(statusCode);
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl, UriKind.Absolute)
        };
        using var shelteredClient = new ShelteredClient(httpClient);

        Assert.Multiple(() =>
        {
            const string expectedMessage = "statusCode is not OK.";
            Assert.That(async () =>
            {
                _ = await shelteredClient.GetAnimalByIdAsync(id, CancellationTokenSource.Token);
            }, Throws.TypeOf<HttpRequestException>().With.Message.EqualTo(expectedMessage));
            Assert.That(handler.GetMatchCount(request), Is.EqualTo(1));
            Assert.That(() => handler.VerifyNoOutstandingExpectation(), Throws.Nothing);
        });
    }

    [Test]
    public void GetAnimalByIdAsync__Should_throw_an_http_request_exception_When_the_deserialized_json_model_is_null()
    {
        const string baseUrl = "http://localhost:5108";
        var id = Guid.NewGuid();
        using var handler = new MockHttpMessageHandler();
        var request = handler
            .Expect($"{baseUrl}/animal/{id}")
            .Respond(HttpStatusCode.OK, JsonContent.Create<AnimalModel?>(null));
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl, UriKind.Absolute)
        };
        using var shelteredClient = new ShelteredClient(httpClient);

        Assert.Multiple(() =>
        {
            const string expectedMessage = "The deserialized json AnimalModel was null.";
            Assert.That(async () =>
            {
                _ = await shelteredClient.GetAnimalByIdAsync(id, CancellationTokenSource.Token);
            }, Throws.TypeOf<HttpRequestException>().With.Message.EqualTo(expectedMessage));
            Assert.That(handler.GetMatchCount(request), Is.EqualTo(1));
            Assert.That(() => handler.VerifyNoOutstandingExpectation(), Throws.Nothing);
        });
    }

    [Test]
    public void GetAnimalByIdAsync__Should_not_throw_an_exception_When_the_response_status_code_is_200_ok_and_the_body_contains_a_valid_json_animal_model()
    {
        const string baseUrl = "http://localhost:5108";
        var id = Guid.NewGuid();
        var animalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat,
            Sex = AnimalSex.Female
        };
        using var handler = new MockHttpMessageHandler();
        var request = handler
            .Expect($"{baseUrl}/animal/{id}")
            .Respond(HttpStatusCode.OK, JsonContent.Create(animalModel));
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl, UriKind.Absolute)
        };
        using var shelteredClient = new ShelteredClient(httpClient);

        Assert.Multiple(() =>
        {
            Assert.That(async () =>
            {
                _ = await shelteredClient.GetAnimalByIdAsync(id, CancellationTokenSource.Token);
            }, Throws.Nothing);
            Assert.That(handler.GetMatchCount(request), Is.EqualTo(1));
            Assert.That(() => handler.VerifyNoOutstandingExpectation(), Throws.Nothing);
        });
    }

    [Test]
    public async Task GetAnimalByIdAsync__Should_return_the_animal_model()
    {
        const string baseUrl = "http://localhost:5108";
        var id = Guid.NewGuid();
        var animalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat,
            Sex = AnimalSex.Female
        };
        using var handler = new MockHttpMessageHandler();
        var request = handler
            .Expect($"{baseUrl}/animal/{id}")
            .Respond(HttpStatusCode.OK, JsonContent.Create(animalModel));
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl, UriKind.Absolute)
        };
        using var shelteredClient = new ShelteredClient(httpClient);

        var actual = await shelteredClient.GetAnimalByIdAsync(id, CancellationTokenSource.Token);

        Assert.Multiple(() =>
        {
            Assert.That(actual, Is.EqualTo(animalModel));
            Assert.That(handler.GetMatchCount(request), Is.EqualTo(1));
            Assert.That(() => handler.VerifyNoOutstandingExpectation(), Throws.Nothing);
        });
    }

    private static IEnumerable<TestCaseData> ListAnimalsAsync__InvalidResponseStatusCodeSource()
    {
        yield return new TestCaseData(HttpStatusCode.NoContent);
        yield return new TestCaseData(HttpStatusCode.BadRequest);
        yield return new TestCaseData(HttpStatusCode.NotFound);
        yield return new TestCaseData(HttpStatusCode.InternalServerError);
    }

    [Test]
    [TestCaseSource(nameof(ListAnimalsAsync__InvalidResponseStatusCodeSource))]
    public void ListAnimalsAsync__Should_throw_an_http_request_exception_When_the_response_status_code_is_invalid(HttpStatusCode statusCode)
    {
        const string baseUrl = "http://localhost:5108";
        using var handler = new MockHttpMessageHandler();
        var request = handler
            .Expect($"{baseUrl}/animal")
            .Respond(statusCode);
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl, UriKind.Absolute)
        };
        using var shelteredClient = new ShelteredClient(httpClient);

        Assert.Multiple(() =>
        {
            const string expectedMessage = "statusCode is not OK.";
            Assert.That(async () =>
            {
                _ = await shelteredClient.ListAnimalsAsync(CancellationTokenSource.Token);
            }, Throws.TypeOf<HttpRequestException>().With.Message.EqualTo(expectedMessage));
            Assert.That(handler.GetMatchCount(request), Is.EqualTo(1));
            Assert.That(() => handler.VerifyNoOutstandingExpectation(), Throws.Nothing);
        });
    }

    [Test]
    public void ListAnimalsAsync__Should_throw_an_http_request_exception_When_the_response_json_body_is_null()
    {
        const string baseUrl = "http://localhost:5108";
        using var handler = new MockHttpMessageHandler();
        var request = handler
            .Expect($"{baseUrl}/animal")
            .Respond(HttpStatusCode.OK, JsonContent.Create<List<AnimalModel>?>(null));
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl, UriKind.Absolute)
        };
        using var shelteredClient = new ShelteredClient(httpClient);

        Assert.Multiple(() =>
        {
            const string expectedMessage = "The deserialized json Dictionary was null.";
            Assert.That(async () =>
            {
                _ = await shelteredClient.ListAnimalsAsync(CancellationTokenSource.Token);
            }, Throws.TypeOf<HttpRequestException>().With.Message.EqualTo(expectedMessage));
            Assert.That(handler.GetMatchCount(request), Is.EqualTo(1));
            Assert.That(() => handler.VerifyNoOutstandingExpectation(), Throws.Nothing);
        });
    }

    [Test]
    public void ListAnimalsAsync__Should_not_throw_an_exception_When_the_response_status_code_is_200_ok_with_a_json_body_of_a_list_of_animals()
    {
        const string baseUrl = "http://localhost:5108";
        var id = Guid.NewGuid();
        var idsToAnimals = new Dictionary<Guid, AnimalModel>
        {
            {
                id,
                new()
                {
                    Name = "Lucy",
                    Kind = AnimalKind.Cat,
                    Sex = AnimalSex.Female
                }
            }
        };
        using var handler = new MockHttpMessageHandler();
        var request = handler
            .Expect($"{baseUrl}/animal")
            .Respond(HttpStatusCode.OK, JsonContent.Create(idsToAnimals));
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl, UriKind.Absolute)
        };
        using var shelteredClient = new ShelteredClient(httpClient);

        Assert.Multiple(() =>
        {
            Assert.That(async () =>
            {
                _ = await shelteredClient.ListAnimalsAsync(CancellationTokenSource.Token);
            }, Throws.Nothing);
            Assert.That(handler.GetMatchCount(request), Is.EqualTo(1));
            Assert.That(() => handler.VerifyNoOutstandingExpectation(), Throws.Nothing);
        });
    }

    [Test]
    public async Task ListAnimalsAsync__Should_return_a_list_of_animals_When_the_response_status_code_is_200_ok_with_a_json_body_of_a_list_of_animals()
    {
        const string baseUrl = "http://localhost:5108";
        var id = Guid.NewGuid();
        var idsToAnimals = new Dictionary<Guid, AnimalModel>
        {
            {
                id,
                new()
                {
                    Name = "Lucy",
                    Kind = AnimalKind.Cat,
                    Sex = AnimalSex.Female
                }
            }
        };
        using var handler = new MockHttpMessageHandler();
        var request = handler
            .Expect($"{baseUrl}/animal")
            .Respond(HttpStatusCode.OK, JsonContent.Create(idsToAnimals));
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl, UriKind.Absolute)
        };
        using var shelteredClient = new ShelteredClient(httpClient);

        var actual = await shelteredClient.ListAnimalsAsync(CancellationTokenSource.Token);

        Assert.Multiple(() =>
        {
            Assert.That(actual, Is.EquivalentTo(idsToAnimals));
            Assert.That(handler.GetMatchCount(request), Is.EqualTo(1));
            Assert.That(() => handler.VerifyNoOutstandingExpectation(), Throws.Nothing);
        });
    }

    private static IEnumerable<TestCaseData> UpdateAnimalByIdAsync__InvalidResponseStatusCodeSource()
    {
        yield return new TestCaseData(HttpStatusCode.OK);
        yield return new TestCaseData(HttpStatusCode.BadRequest);
        yield return new TestCaseData(HttpStatusCode.InternalServerError);
    }

    [Test]
    [TestCaseSource(nameof(UpdateAnimalByIdAsync__InvalidResponseStatusCodeSource))]
    public void UpdateAnimalByIdAsync__Should_throw_an_http_request_exception_When_the_response_status_code_is_invalid(HttpStatusCode statusCode)
    {
        const string baseUrl = "http://localhost:5108";
        var id = Guid.NewGuid();
        var animalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat,
            Sex = AnimalSex.Female
        };
        using var handler = new MockHttpMessageHandler();
        var request = handler
            .Expect($"{baseUrl}/animal/{id}")
                .WithJsonContent(animalModel)
            .Respond(statusCode);
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl, UriKind.Absolute)
        };
        using var shelteredClient = new ShelteredClient(httpClient);

        Assert.Multiple(() =>
        {
            const string expectedMessage = "statusCode is neither NoContent nor NotFound.";
            Assert.That(async () =>
            {
                _ = await shelteredClient.UpdateAnimalByIdAsync(id, animalModel, CancellationTokenSource.Token);
            }, Throws.TypeOf<HttpRequestException>().With.Message.EqualTo(expectedMessage));
            Assert.That(handler.GetMatchCount(request), Is.EqualTo(1));
            Assert.That(() => handler.VerifyNoOutstandingExpectation(), Throws.Nothing);
        });
    }

    private static IEnumerable<TestCaseData> UpdateAnimalByIdAsync__ValidResponseStatusCodeSource()
    {
        yield return new TestCaseData(HttpStatusCode.NoContent);
        yield return new TestCaseData(HttpStatusCode.NotFound);
    }

    [Test]
    [TestCaseSource(nameof(UpdateAnimalByIdAsync__ValidResponseStatusCodeSource))]
    public void UpdateAnimalByIdAsync__Should_not_throw_an_exception_When_the_response_status_code_is_valid(HttpStatusCode statusCode)
    {
        const string baseUrl = "http://localhost:5108";
        var id = Guid.NewGuid();
        var animalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat,
            Sex = AnimalSex.Female
        };
        using var handler = new MockHttpMessageHandler();
        var request = handler
            .Expect($"{baseUrl}/animal/{id}")
                .WithJsonContent(animalModel)
            .Respond(statusCode);
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl, UriKind.Absolute)
        };
        using var shelteredClient = new ShelteredClient(httpClient);

        Assert.Multiple(() =>
        {
            Assert.That(async () =>
            {
                _ = await shelteredClient.UpdateAnimalByIdAsync(id, animalModel, CancellationTokenSource.Token);
            }, Throws.Nothing);
            Assert.That(handler.GetMatchCount(request), Is.EqualTo(1));
            Assert.That(() => handler.VerifyNoOutstandingExpectation(), Throws.Nothing);
        });
    }

    [Test]
    public async Task UpdateAnimalByIdAsync__Should_return_true_When_the_response_status_code_is_204_no_content()
    {
        const string baseUrl = "http://localhost:5108";
        var id = Guid.NewGuid();
        var animalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat,
            Sex = AnimalSex.Female
        };
        using var handler = new MockHttpMessageHandler();
        var request = handler
            .Expect($"{baseUrl}/animal/{id}")
                .WithJsonContent(animalModel)
            .Respond(HttpStatusCode.NoContent);
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl, UriKind.Absolute)
        };
        using var shelteredClient = new ShelteredClient(httpClient);

        var actual = await shelteredClient.UpdateAnimalByIdAsync(id, animalModel, CancellationTokenSource.Token);

        Assert.Multiple(() =>
        {
            Assert.That(actual, Is.True);
            Assert.That(handler.GetMatchCount(request), Is.EqualTo(1));
            Assert.That(() => handler.VerifyNoOutstandingExpectation(), Throws.Nothing);
        });
    }

    [Test]
    public async Task UpdateAnimalByIdAsync__Should_return_false_When_the_response_status_code_is_404_not_found()
    {
        const string baseUrl = "http://localhost:5108";
        var id = Guid.NewGuid();
        var animalModel = new AnimalModel
        {
            Name = "Lucy",
            Kind = AnimalKind.Cat,
            Sex = AnimalSex.Female
        };
        using var handler = new MockHttpMessageHandler();
        var request = handler
            .Expect($"{baseUrl}/animal/{id}")
                .WithJsonContent(animalModel)
            .Respond(HttpStatusCode.NotFound);
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl, UriKind.Absolute)
        };
        using var shelteredClient = new ShelteredClient(httpClient);

        var actual = await shelteredClient.UpdateAnimalByIdAsync(id, animalModel, CancellationTokenSource.Token);

        Assert.Multiple(() =>
        {
            Assert.That(actual, Is.False);
            Assert.That(handler.GetMatchCount(request), Is.EqualTo(1));
            Assert.That(() => handler.VerifyNoOutstandingExpectation(), Throws.Nothing);
        });
    }
}
