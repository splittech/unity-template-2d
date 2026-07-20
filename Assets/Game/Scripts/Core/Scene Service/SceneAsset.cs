using System;
using Eflatun.SceneReference;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace Game.Core
{
    [CreateAssetMenu(fileName = "Scene Meta Asset", menuName = "Game/Configs/Scene Service/Scene Meta Asset", order = 0)]
    public class SceneMetaAsset : ScriptableObject
    {
        [SerializeField] private SceneReference sceneReference;
        [SerializeField] private bool initial;
        [SerializeField] private bool persistent;

        public SceneReference SceneReference => sceneReference;
        public bool Initial => initial;
        public bool Persistent => persistent;

        public static void Validate(SceneMetaAsset sceneAsset)
        {
            if (sceneAsset == null)
                throw new InvalidOperationException("Scene asset cannot be null.");

            string editorMessage = "";
#if UNITY_EDITOR
            editorMessage = $" Asset path: {AssetDatabase.GetAssetPath(sceneAsset)}";
#endif
            if (sceneAsset.sceneReference == null)
            {
                string message = "Scene asset has no scene reference.";
                throw new InvalidOperationException(message + editorMessage);
            }
        }
    }
}