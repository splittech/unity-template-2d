namespace Game.Core
{
    public abstract class GameLogger
    {
        protected readonly LoggingServiceConfig.ChannelConfig _config;

        protected GameLogger(LoggingServiceConfig.ChannelConfig config)
        {
            _config = config;
        }

        public abstract void Log(string message);
    }
}
