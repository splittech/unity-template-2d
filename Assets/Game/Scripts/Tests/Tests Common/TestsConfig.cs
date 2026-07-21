using Game.Core;
using UnityEditor;
using UnityEngine;

namespace Game.Tests.Common
{
    [CreateAssetMenu(fileName = "Tests Config", menuName = "Game/Tests/Tests Config", order = 0)]
    public class TestsConfig : ScriptableObject
    {
        private const string TestsConfigAssetPath = "Assets/Game/Scripts/Tests/Configs/Tests Config.asset";

        [SerializeField] private SceneServiceConfig sceneServiceConfig;
        public SceneServiceConfig SceneServiceConfig => sceneServiceConfig;

        public static TestsConfig Get()
        {
            return AssetDatabase.LoadAssetAtPath<TestsConfig>(TestsConfigAssetPath);
        }
    }
}
