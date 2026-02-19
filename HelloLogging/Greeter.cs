namespace HelloLogging;

public class Greeter(ILogger<Greeter> logger)
{
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
        logger.LogInformation($"string-interpolation: Hello, {whoToGreet}");
        logger.LogInformation("template: Hello, {parameter-whoToGreet}", whoToGreet);
        return $"Hello, {whoToGreet}!";
    }
}