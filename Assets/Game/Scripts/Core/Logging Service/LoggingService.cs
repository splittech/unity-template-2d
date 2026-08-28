using System.Collections.Generic;

namespace Game.Core
{
    public class LoggingService : ILoggingService
    {
        private readonly LoggingServiceConfig _config;
        private readonly LoggerFactory _loggerFactory;

        private readonly Dictionary<LoggingChannel, GameLogger> _loggers = new();

        public LoggingService(LoggingServiceConfig config, LoggerFactory loggerFactory)
        {
            _config = config;
            _loggerFactory = loggerFactory;

            CreateLoggers();
        }

        public GameLogger GetLogger(LoggingChannel channel)
        {
            if (!_loggers.TryGetValue(channel, out var logger))
                return null;

            return logger;
        }

        private void CreateLoggers()
        {
            foreach (var channel in _config.LogChannels)
            {
                LoggingChannel loggingChannel = channel.Key;
                LogChannelConfig channelConfig = channel.Value;

                _loggers.Add(loggingChannel, _loggerFactory.Create(channelConfig));
            }
        }
    }
}