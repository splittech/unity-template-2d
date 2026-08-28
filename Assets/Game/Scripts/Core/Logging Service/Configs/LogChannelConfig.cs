using System;
using UnityEngine;

namespace Game.Core
{
    [Serializable]
    public class LogChannelConfig
    {
        [SerializeField] private bool _muted;
        [SerializeField] private string channelPrefix = "";
        [SerializeField] private Color prefixColor = Color.white;
        [SerializeField] private LoggerTypes _loggerTypes;

        public bool Muted => _muted;
        public string ChannelPrefix => channelPrefix;
        public Color PrefixColor => prefixColor;
        public LoggerTypes LoggerTypes => _loggerTypes;
    }
}
