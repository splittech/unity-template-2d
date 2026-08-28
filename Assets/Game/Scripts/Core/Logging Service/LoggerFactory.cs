using System.Collections.Generic;

namespace Game.Core
{
    public class LoggerFactory
    {
        public GameLogger Create(LogChannelConfig channelConfig)
        {
            List<GameLogger> loggers = new();

            if (channelConfig.LoggerTypes.HasFlag(LoggerTypes.UnityLogger))
                loggers.Add(new UnityLogger(channelConfig));

            if (channelConfig.LoggerTypes.HasFlag(LoggerTypes.FileLogger))
                loggers.Add(new FileLogger(channelConfig));

            if (channelConfig.LoggerTypes.HasFlag(LoggerTypes.ServerLogger))
                loggers.Add(new ServerLogger(channelConfig));

            if (loggers.Count == 0)
                return new EmptyLogger(channelConfig);

            if (loggers.Count == 1)
                return loggers[0];

            return new CompositeLogger(channelConfig, loggers);
        }
    }
}