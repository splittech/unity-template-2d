using System;
using Eflatun.SceneReference;
using UnityEngine;

namespace Game.Core
{
    [CreateAssetMenu(fileName = "Scene Meta Asset", menuName = "Game/Meta Assets/Scene Service/Scene Meta Asset", order = 0)]
    public class SceneMetaAsset : ScriptableObject
    {
        [SerializeField] private SceneReference _sceneReference;
        [SerializeField] private bool _initial;
        [SerializeField] private bool _persistent;

        public SceneReference SceneReference => _sceneReference;
        public bool Initial => _initial;
        public bool Persistent => _persistent;

        public static void Validate(SceneMetaAsset sceneAsset)
        {
            if (sceneAsset == null)
            {
                throw new InvalidOperationException(
                    $"SceneMetaAsset 'null': Scene asset can't be null.");
            }

            if (sceneAsset._sceneReference == null)
            {
                throw new InvalidOperationException(
                    $"SceneMetaAsset '{sceneAsset.name}': Scene asset has no scene reference.");
            }
        }
    }
}