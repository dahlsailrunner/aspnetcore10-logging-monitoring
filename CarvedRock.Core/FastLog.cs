using Microsoft.Extensions.Logging;

namespace CarvedRock.Core;

public partial class FastLog
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Calling repository.")]
    public static partial void CallingRepository(ILogger logger);
}