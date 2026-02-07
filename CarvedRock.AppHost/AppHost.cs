using Projects;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<HelloLogging>("scratchpad");

builder.Build().Run();
