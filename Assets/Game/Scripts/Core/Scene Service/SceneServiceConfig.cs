using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace Game.Core.SceneService
{
    [CreateAssetMenu(fileName = "Scene Service Config", menuName = "Game/Configs/Scene Service/Scene Service Config", order = 0)]
    public class SceneServiceConfig : ScriptableObject
    {
        [SerializeField] private SceneMetaAsset coreScene;
        [SerializeField] private SceneMetaAsset mainMenuScene;
        [SerializeField] private SceneMetaAsset gameplayScene;

        private readonly List<SceneMetaAsset> allScenes = new();

        public void RecreateAllScenesList()
        {
            allScenes.Clear();

            allScenes.Add(coreScene);
            allScenes.Add(mainMenuScene);
            allScenes.Add(gameplayScene);

            ValidateAllScenesList();
        }

        private void ValidateAllScenesList()
        {
            string editorMessage = "";
#if UNITY_EDITOR
            editorMessage = $" Config path: {AssetDatabase.GetAssetPath(this)}";
#endif
            try
            {
                allScenes.ForEach(scene => SceneMetaAsset.Validate(scene));
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException(ex.Message + editorMessage);
            }

            if (allScenes.Distinct().Count() != allScenes.Count)
            {
                string message = "Scene list contains duplicates.";
                throw new InvalidOperationException(message + editorMessage);
            }

            Debug.Log("Scene list validation is successful.");
        }

        public SceneMetaAsset GetCoreScene()
        {
            return coreScene;
        }

        public List<SceneMetaAsset> GetAllScenes()
        {
            return new List<SceneMetaAsset>(allScenes);
        }

        public List<SceneMetaAsset> GetAllNonPersistentScenes()
        {
            return allScenes.Where(scene => !scene.Persistent && scene != coreScene).ToList();
        }

        public List<SceneMetaAsset> GetAllInitialScenes()
        {
            return allScenes.Where(scene => scene.Initial && scene != coreScene).ToList();
        }

        public List<SceneMetaAsset> GetAllScenesExceptCore()
        {
            return allScenes.Where(scene => scene != coreScene).ToList();
        }
    }
}