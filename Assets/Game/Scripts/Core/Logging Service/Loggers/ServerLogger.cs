namespace Game.Core
{
    public class ServerLogger : GameLogger
    {
        public ServerLogger(LoggingServiceConfig.ChannelConfig config) : base(config) { }

        public override void Log(string message)
        {
            // Pass.
        }
    }
}
