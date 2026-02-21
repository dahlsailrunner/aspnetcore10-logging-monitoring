using CarvedRock.Api;
using CarvedRock.Core;
using CarvedRock.Data;
using CarvedRock.Domain;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddValidatorsFromAssemblyContaining<NewProductValidator>();
builder.Services.AddProblemDetails(opts => opts.CustomizeProblemDetails = CustomizeProblemDetails);
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();

// builder.AddAzureOpenAIClient("kyt-AzureOpenAI", configureSettings: settings =>
// {
//     settings.EnableSensitiveTelemetryData = true;
//     settings.Endpoint = new Uri(builder.Configuration.GetValue<string>("AIConnection:Endpoint")!);
//     settings.Key = builder.Configuration.GetValue<string>("AIConnection:Key")!;
// }).AddChatClient(builder.Configuration.GetValue<string>("AIConnection:Deployment")!);

// Add NuGet package reference to Aspire.OpenAI
// Then add your API key for OpenAI to user secrets for the AIConnection:OpenAIKey value
builder.AddOpenAIClient("kyt-OpenAI", configureSettings: settings =>
{
    settings.EnableSensitiveTelemetryData = true;
    settings.Key = builder.Configuration.GetValue<string>("AIConnection:OpenAIKey");
}).AddChatClient(builder.Configuration.GetValue<string>("AIConnection:OpenAIModel"));

builder.Services.AddHttpContextAccessor();

var authority = builder.Configuration.GetValue<string>("Auth:Authority");
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = authority;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = "email",
            //RoleClaimType = "role",
            ValidateAudience = false
        };
    });
builder.Services.AddTransient<IClaimsTransformation, AdminClaimsTransformation>();

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerOptions>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IProductLogic, ProductLogic>();
builder.AddNpgsqlDbContext<LocalContext>("CarvedRockPostgres");
builder.Services.AddScoped<ICarvedRockRepository, CarvedRockRepository>();

var app = builder.Build();
app.MapDefaultEndpoints();
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    SetupDevelopment(app);
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireAuthorization();

app.Run();

static void SetupDevelopment(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<LocalContext>();
        context.MigrateAndCreateData();
    }
    ;

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.OAuthClientId("interactive.public");
        options.OAuthAppName("CarvedRock API");
        options.OAuthUsePkce();
    });
}

static void CustomizeProblemDetails(ProblemDetailsContext context)
{
    context.ProblemDetails.Detail = "Provide the instance value when contacting us for support";
    context.ProblemDetails.Instance = Activity.Current?.RootId;
}

record AIConnection(string Endpoint, string Key, string Deployment);

