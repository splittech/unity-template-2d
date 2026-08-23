using System.Collections.Generic;

namespace Game.Core
{
    public class CompositeLogger : GameLogger
    {
        private readonly List<GameLogger> _loggers;

        public CompositeLogger(LoggingServiceConfig.ChannelConfig config, List<GameLogger> loggers) : base(config)
        {
            _loggers = loggers;
        }

        public override void Log(string message)
        {
            if (_config.Muted)
                return;

            _loggers.ForEach(logger => logger.Log(message));
        }
    }
}
