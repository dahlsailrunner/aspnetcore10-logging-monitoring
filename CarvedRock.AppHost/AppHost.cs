using Projects;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<HelloLogging>("scratchpad");

var carvedrockdb = builder.AddPostgres("postgres")
                          .AddDatabase("CarvedRockPostgres");

var idsrv = builder.AddProject<Duende_IdentityServer_Demo>("idsrv");

var api = builder.AddProject<CarvedRock_Api>("api")
    .WithReference(carvedrockdb)
    .WaitFor(carvedrockdb)
    .WithEnvironment("Auth__Authority", idsrv.GetEndpoint("https"));

var mailpit = builder.AddMailPit("smtp");

builder.AddProject<CarvedRock_WebApp>("webapp")
    .WithReference(api)
    .WithReference(mailpit)
    .WaitFor(mailpit)
    .WaitFor(api)
    .WithEnvironment("Auth__Authority", idsrv.GetEndpoint("https"));

var mcp = builder.AddProject<CarvedRock_Mcp>("mcp")
    .WithReference(api)
    .WithEnvironment("AuthServer", idsrv.GetEndpoint("https"));

api.WithReference(mcp);  // add reference to mcp server from API

builder.AddMcpInspector("mcp-inspector")
    .WithMcpServer(mcp, path: "");


builder.Build().Run();
