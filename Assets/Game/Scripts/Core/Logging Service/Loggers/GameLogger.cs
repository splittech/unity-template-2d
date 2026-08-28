namespace Game.Core
{
    public abstract class GameLogger
    {
        protected readonly LogChannelConfig _config;

        protected GameLogger(LogChannelConfig config)
        {
            _config = config;
        }

        public abstract void Log(string message);
    }
}
