namespace Game.Core.Tests.Doubles
{
    public class MockLoggingService : ILoggingService
    {
        private readonly EmptyLogger emptyLogger = new(null);

        public GameLogger GetLogger(LoggingChannel loggerChannel)
        {
            return emptyLogger;
        }
    }
}
