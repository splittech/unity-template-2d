using System;
using System.Collections.Generic;
using Alchemy.Serialization;
using UnityEngine;

namespace Game.Core
{
    [Flags]
    public enum LoggerTypes
    {
        UnityLogger = 1 << 0,
        FileLogger = 1 << 1,
        ServerLogger = 1 << 2,
    }

    public enum LoggingChannel
    {
        ApplicationService,
        SceneService,
        AudioService,
        AddressablesService
    }

    [CreateAssetMenu(fileName = "Logging Service Config", menuName = "Game/Configs/Logging Service Config", order = 0)]
    [AlchemySerialize]
    public partial class LoggingServiceConfig : ScriptableObject
    {
        [AlchemySerializeField, NonSerialized] private Dictionary<LoggingChannel, ChannelConfig> _channels = new();

        public Dictionary<LoggingChannel, ChannelConfig> Channels => _channels;

        public class ChannelConfig
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
}
