using System.Reflection;
using Scotch;
using Stripe;
using Xunit.Abstractions;

namespace PayDotNet.Core.Stripe.Tests.StripeTests;

public static class StripeTestHelper
{
    public const ScotchMode DefaultMode = ScotchMode.Replaying;
    private const string ApiKey = "sk_test_51IQViRJUAL06t0UNemg2YVbfQ150HFQm7MdQmBjznqR0lUD9QR65dTfBHIoP11rBdU9I8QYmmvpzN3Y6bgG4XGQ500BPIpaZwU";

    public static void SetupStripeRecording(this ITestOutputHelper testOutputHelper, ScotchMode mode = DefaultMode)
    {
        var type = testOutputHelper.GetType();
        var testMember = type.GetField("test", BindingFlags.Instance | BindingFlags.NonPublic);
        ITest? test = (ITest)testMember.GetValue(testOutputHelper);
        SetupStripeRecording(test!.TestCase.TestMethod.Method.Name, mode);
    }

    public static void SetupStripeRecording(string testName, ScotchMode mode = DefaultMode)
    {
        HttpClient httpClient = HttpClients.NewHttpClient($"../../../ScotchCassettes/{testName}.json", mode);
        StripeConfiguration.StripeClient =
            new StripeClient(ApiKey, httpClient: new SystemNetHttpClient(httpClient, appInfo: StripePaymentProcessorService.AppInfo));
    }
}