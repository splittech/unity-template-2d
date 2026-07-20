using System;
using System.Collections.Generic;
using System.Linq;
using Alchemy.Inspector;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;

#endif

namespace Game.Core
{
    [CreateAssetMenu(fileName = "Scene Service Config", menuName = "Game/Configs/Scene Service/Scene Service Config", order = 0)]
    public class SceneServiceConfig : ScriptableObject
    {
        [SerializeField] private SceneMetaAsset coreScene;
        [SerializeField] private SceneMetaAsset mainMenuScene;
        [SerializeField] private SceneMetaAsset gameplayScene;

        private readonly List<SceneMetaAsset> allScenes = new();

        public SceneMetaAsset CoreScene => coreScene;
        public SceneMetaAsset MainMenuScene => mainMenuScene;
        public SceneMetaAsset GameplayScene => gameplayScene;
        public List<SceneMetaAsset> AllScenes => allScenes.ToList();

        public void RecreateAllScenesList()
        {
            allScenes.Clear();

            allScenes.Add(coreScene);
            allScenes.Add(mainMenuScene);
            allScenes.Add(gameplayScene);

            ValidateAllScenesList();
        }

        public List<SceneMetaAsset> GetAllScenes()
        {
            return allScenes.ToList();
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

#if UNITY_EDITOR
        [Button]
        private void OpenOnlyCoreScene()
        {
            RecreateAllScenesList();

            GetAllScenes().ForEach(sceneAsset =>
            {
                Scene scene = SceneManager.GetSceneByName(sceneAsset.SceneReference.Name);
                EditorSceneManager.CloseScene(scene, true);
            });

            EditorSceneManager.OpenScene(coreScene.SceneReference.Path);
        }
#endif
    }
}