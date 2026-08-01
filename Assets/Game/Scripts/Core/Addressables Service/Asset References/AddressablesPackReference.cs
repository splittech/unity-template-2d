using System;
using UnityEngine.AddressableAssets;

namespace Game.Core
{
    [Serializable]
    public sealed class AddressablesPackReference : AssetReferenceT<AddressablesPack>
    {
        public AddressablesPackReference(string guid) : base(guid) { }
    }
}
