using IntelligentPlant.IndustrialAppStore.CommandLine;
using IntelligentPlant.IndustrialAppStore.Client;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>()
    .Build();

var services = new ServiceCollection();

services.AddIndustrialAppStoreCliServices(options => configuration.GetSection("IAS").Bind(options));

var provider = services.BuildServiceProvider();

using var scope = provider.CreateScope();

var sessionManager = scope.ServiceProvider.GetRequiredService<IndustrialAppStoreSessionManager>();
try {
    await sessionManager.SignInAsync((request, ct) => {
        Console.WriteLine($"Please sign in by visiting {request.VerificationUri} and entering the following code: {request.UserCode}");
        Console.WriteLine();
        return default;
    });
}
catch (OperationCanceledException) {
    Console.WriteLine("Sign-in cancelled.");
    return 1;
}

var client = scope.ServiceProvider.GetRequiredService<IndustrialAppStoreHttpClient>();
var userInfo = await client.UserInfo.GetUserInfoAsync();

Console.WriteLine("User Information:");
Console.WriteLine();
Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(userInfo, new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web) { WriteIndented = true }));

return 0;
