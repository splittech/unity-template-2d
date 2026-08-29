using Eflatun.SceneReference;
using UnityEngine;

namespace Game.Core
{
    [CreateAssetMenu(fileName = "Scene Meta Asset", menuName = "Game/Meta Assets/Scene Service/Scene Meta Asset", order = 0)]
    public class SceneMetaAsset : ScriptableObject
    {
        public SceneReference SceneReference;
        public bool Initial;
        public bool Persistent;
    }
}