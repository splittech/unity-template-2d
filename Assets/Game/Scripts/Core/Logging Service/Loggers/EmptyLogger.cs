namespace Game.Core
{
    public class EmptyLogger : GameLogger
    {
        public EmptyLogger(LoggingServiceConfig.ChannelConfig config) : base(config) { }

        public override void Log(string message)
        {
            // Pass.
        }
    }
}
