using HelloLogging;

var builder = WebApplication.CreateBuilder(args);
//https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging 
// builder.Logging.ClearProviders();
// builder.Logging.AddDebug();
// builder.Logging.AddConsole();

builder.AddServiceDefaults();
builder.Services.AddScoped<Greeter>();

var app = builder.Build();
app.MapDefaultEndpoints();

app.MapGet("/", (ILogger<Program> logger, Greeter greeter) =>
{
    logger.LogInformation("Got into root request.");
    return greeter.Greet("Logging World");
});

app.MapGet("/uh-oh", (Greeter greeter) =>
{
    return greeter.Greet("trouble");
});

app.MapGet("/{whoToGreet}", (string whoToGreet, Greeter greeter) =>
{
    return greeter.Greet(whoToGreet);
});

app.Run();
