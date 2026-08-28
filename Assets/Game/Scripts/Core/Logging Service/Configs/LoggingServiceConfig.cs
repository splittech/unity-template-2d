using System;
using System.Collections.Generic;
using Alchemy.Serialization;
using UnityEngine;

#if UNITY_EDITOR

using Alchemy.Inspector;
using UnityEditor;

#endif

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

    [AlchemySerialize]
    [CreateAssetMenu(fileName = "Logging Service Config", menuName = "Game/Configs/Logging Service Config", order = 0)]
    public partial class LoggingServiceConfig : ScriptableObject
    {
        [AlchemySerializeField, NonSerialized]
        private Dictionary<LoggingChannel, LogChannelConfig> _logChannels = new();

        public Dictionary<LoggingChannel, LogChannelConfig> LogChannels => _logChannels;

#if UNITY_EDITOR
        [OnInspectorDisable]
        private void SaveInspectorChanges()
        {
            // ALCHEMY BUG FIX
            // Alchemy's reflection-based dictionary field does not reliably mark
            // ScriptableObjects dirty after adding an entry. Flush its generated
            // JSON explicitly before the inspector releases this asset.
            ((ISerializationCallbackReceiver)this).OnBeforeSerialize();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
        }
#endif
    }
}
