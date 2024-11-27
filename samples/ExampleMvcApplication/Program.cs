using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Content security policy is defined in csp.json
builder.Configuration.AddJsonFile("csp.json", optional: true, reloadOnChange: true);
builder.Configuration.AddJsonFile($"csp.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

builder.Services.AddCustomHeaders();
builder.Services.AddContentSecurityPolicy();

builder.Services.AddIndustrialAppStoreAuthentication(options => {
    // Bind the settings from the app configuration to the Industrial App Store 
    // authentication options.
    builder.Configuration.GetSection("IAS").Bind(options);

    // Redirect to our login page when an authentication challenge is issued.
    options.LoginPath = new PathString("/Account/Login");

    // The UseCookieSessionIdGenerator extension method configures the SessionIdGenerator
    // property to store a persistent device ID cookie in the calling user agent, so
    // that logins from the same browser will always use the same session ID. If
    // SessionIdGenerator is not configured, a new session ID will be generated for
    // every login.
    //options.UseCookieSessionIdGenerator();
});

if (builder.Configuration.GetValue<bool>("IAS:UseExternalAuthentication")) {
    // App is configured to use an authentication provider other than the Industrial
    // App Store, so we will use IIS to handle Windows authentication for us.
    builder.Services.AddAuthentication(Microsoft.AspNetCore.Server.IISIntegration.IISDefaults.AuthenticationScheme);
}

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson();

builder.Services.AddOpenTelemetry()
    .ConfigureResource(otel => otel.AddDefaultService())
    // Export telemetry signals using OTLP (OpenTelemetry Protocol). Exporter settings can be
    // configured in appsettings.json. See https://github.com/wazzamatazz/opentelemetry-extensions#configuring-a-multi-signal-opentelemetry-protocol-otlp-exporter 
    // for more information on configuring the OTLP exporter.
    .AddOtlpExporter(builder.Configuration)
    .WithTracing(otel => otel
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation())
    .WithMetrics(otel => otel
        .AddRuntimeInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation());

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}
else {
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCustomHeaders();
app.UseContentSecurityPolicy();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();
