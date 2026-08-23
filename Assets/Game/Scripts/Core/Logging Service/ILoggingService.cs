namespace Game.Core
{
    public interface ILoggingService
    {
        GameLogger GetLogger(LoggingChannel channel);
    }
}