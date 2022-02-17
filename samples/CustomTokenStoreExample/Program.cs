var builder = WebApplication.CreateBuilder(args);

AppBuilderUtils.ConfigureServices(builder.Configuration, builder.Services);
var app = builder.Build();

AppBuilderUtils.ConfigureApp(app, app.Environment);
app.Run();
