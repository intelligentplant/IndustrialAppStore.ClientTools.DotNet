var builder = WebApplication.CreateBuilder(args);

// Content security policy is defined in csp.json.
builder.Configuration.AddJsonFile("csp.json", optional: true, reloadOnChange: true);

AppBuilderUtils.ConfigureServices(builder.Configuration, builder.Services);
var app = builder.Build();

AppBuilderUtils.ConfigureApp(app, app.Environment);
app.Run();
