namespace Game.Core
{
    public class FileLogger : GameLogger
    {
        public FileLogger(LoggingServiceConfig.ChannelConfig config) : base(config) { }

        public override void Log(string message)
        {
            // Pass.
        }
    }
}
