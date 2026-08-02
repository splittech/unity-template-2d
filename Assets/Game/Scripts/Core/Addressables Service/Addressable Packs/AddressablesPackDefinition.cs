using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Core
{
    [Serializable]
    public class AddressablesPackDefinition
    {
        [SerializeField] private string _id;
        [SerializeField] private AssetLabelReference _downloadLabel;
        [SerializeField] private AddressablesPackReference _pack;

        public string Id => _id;
        public string DownloadLabel => _downloadLabel?.labelString;
        public AddressablesPackReference Pack => _pack;

        public AddressablesPackDefinition(
            string id,
            AssetLabelReference downloadLabel,
            AddressablesPackReference pack)
        {
            _id = id;
            _downloadLabel = downloadLabel;
            _pack = pack;
        }
    }
}
