using System;
using Eflatun.SceneReference;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

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
                throw new InvalidOperationException("Scene asset cannot be null.");

            string editorMessage = "";
#if UNITY_EDITOR
            editorMessage = $" Asset path: {AssetDatabase.GetAssetPath(sceneAsset)}";
#endif
            if (sceneAsset._sceneReference == null)
            {
                string message = "Scene asset has no scene reference.";
                throw new InvalidOperationException(message + editorMessage);
            }
        }
    }
}