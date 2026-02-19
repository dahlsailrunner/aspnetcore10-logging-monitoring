using HelloLogging;

var builder = WebApplication.CreateBuilder(args);
//https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging 
// builder.Logging.ClearProviders();
// builder.Logging.AddDebug();
// builder.Logging.AddConsole();

builder.AddServiceDefaults();
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = CustomizeProblemDetails;
});

builder.Services.AddScoped<Greeter>();

var app = builder.Build();
app.MapDefaultEndpoints();
app.UseExceptionHandler();

app.MapGet("/", (ILogger<Program> logger, Greeter greeter) =>
{
    logger.LogInformation("Got into root request.");
    return greeter.Greet("Logging World");
});

app.MapGet("/uh-oh", (Greeter greeter) =>
{
    return greeter.Greet("trouble");
});

app.MapGet("/big-trouble", (Greeter greeter) =>
{
    return greeter.Greet("exception");
});

app.MapGet("/{whoToGreet}", (string whoToGreet, Greeter greeter) =>
{
    return greeter.Greet(whoToGreet);
});

app.Run();

void CustomizeProblemDetails(ProblemDetailsContext context)
{
    context.ProblemDetails.Title = "Oops.  This is embarassing.";
    context.ProblemDetails.Detail = context.Exception?.Message;
}