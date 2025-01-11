using System.Threading.Tasks;
using NUnit.Framework;
using EndToEndTests.Common;
using ApiProgram = Api.Program;
using WebProgram = Web.Program;

namespace EndToEndTests;

[SetUpFixture]
internal static class EndToEndHostSetUp
{
    public static BlazorApplicationFactory<WebProgram> Web { get; private set; } = null!;
    public static ApiApplicationFactory<ApiProgram> Api { get; private set; } = null!;

    [OneTimeSetUp]
    public static void HostSetUp()
    {
        Api = new();
        Web = new(Api.CreateClient());
    }

    [OneTimeTearDown]
    public static async Task HostTearDown()
    {
        await Api.DisposeAsync();
        await Web.DisposeAsync();
    }
}
