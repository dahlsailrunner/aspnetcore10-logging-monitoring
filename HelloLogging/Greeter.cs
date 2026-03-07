using CarvedRock.ServiceDefaults;

namespace HelloLogging;

public class Greeter(ILogger<Greeter> logger) //ILoggerFactory loggerFactory) // or just ILogger<Greeter>
{
    // below is only an example - probably best to simply inject ILogger<Greeter>
    //readonly ILogger logger = loggerFactory.CreateLogger("SomeCategory.OfMyOwnChoosing");
    public string Greet(string whoToGreet)
    {
        if (string.Equals(whoToGreet, "exception", StringComparison.InvariantCultureIgnoreCase))
        {
            throw new Exception("Request has failed!");
        }

        if (string.Equals(whoToGreet, "trouble", StringComparison.InvariantCultureIgnoreCase))
        {
            logger.Log(LogLevel.Warning, "Trouble detected.  Use caution.");
        }
        //logger.LogInformation($"string-interpolation: Hello, {whoToGreet}");

        //logger.LogInformation("Hello, {whoToGreet}", whoToGreet);
        Log.Hello(logger, whoToGreet, "888-88-8888".MaskSsn()); // high-performance 
        return $"Hello, {whoToGreet}!";
    }
}