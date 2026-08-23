
using UnityEngine;

namespace Game.Core
{
    public class UnityLogger : GameLogger
    {
        public UnityLogger(LoggingServiceConfig.ChannelConfig config) : base(config) { }

        public override void Log(string message)
        {
            if (_config.Muted)
                return;

            string prefix = _config.ChannelPrefix;
            string color = ColorUtility.ToHtmlStringRGB(_config.PrefixColor);

            Debug.Log($"<color=#{color}><b>[{prefix}]</b></color> {message}");
        }
    }
}
