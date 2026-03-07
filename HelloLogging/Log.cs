namespace HelloLogging;

// https://learn.microsoft.com/en-us/dotnet/core/extensions/logging/high-performance-logging
public static partial class Log
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Got into root request.")]
    public static partial void RootRequest(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Hello, {whoToGreet}: {ssn}")]
    public static partial void Hello(ILogger logger,
        string whoToGreet,
        //[Private]
        string ssn);
}