using Game.Core;
using UnityEditor;
using UnityEngine;

namespace Game.Tests.Common
{
    [CreateAssetMenu(fileName = "Tests Config", menuName = "Game/Tests/Tests Config", order = 0)]
    public class TestConfig : ScriptableObject
    {
        private const string TestsConfigAssetPath = "Assets/Game/Tests/Configs/Tests Config.asset";

        [SerializeField] private SceneServiceConfig sceneServiceConfig;

        public SceneServiceConfig SceneServiceConfig => sceneServiceConfig;

        public static TestConfig Get()
        {
            return AssetDatabase.LoadAssetAtPath<TestConfig>(TestsConfigAssetPath);
        }
    }
}
