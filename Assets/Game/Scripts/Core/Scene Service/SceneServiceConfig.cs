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
        [SerializeField] private SceneMetaAsset _coreScene;
        [SerializeField] private SceneMetaAsset _mainMenuScene;
        [SerializeField] private SceneMetaAsset _gameplayScene;

        private readonly List<SceneMetaAsset> _allScenes = new();

        public SceneMetaAsset CoreScene => _coreScene;
        public SceneMetaAsset MainMenuScene => _mainMenuScene;
        public SceneMetaAsset GameplayScene => _gameplayScene;
        public List<SceneMetaAsset> AllScenes => _allScenes.ToList();

        public void RecreateAllScenesList()
        {
            _allScenes.Clear();

            _allScenes.Add(_coreScene);
            _allScenes.Add(_mainMenuScene);
            _allScenes.Add(_gameplayScene);

            ValidateAllScenesList();
        }

        public List<SceneMetaAsset> GetAllScenes()
        {
            return _allScenes.ToList();
        }

        public List<SceneMetaAsset> GetAllNonPersistentScenes()
        {
            return _allScenes.Where(scene => !scene.Persistent && scene != _coreScene).ToList();
        }

        public List<SceneMetaAsset> GetAllInitialScenes()
        {
            return _allScenes.Where(scene => scene.Initial && scene != _coreScene).ToList();
        }

        public List<SceneMetaAsset> GetAllScenesExceptCore()
        {
            return _allScenes.Where(scene => scene != _coreScene).ToList();
        }

        private void ValidateAllScenesList()
        {
            string editorMessage = "";
#if UNITY_EDITOR
            editorMessage = $" Config path: {AssetDatabase.GetAssetPath(this)}";
#endif
            try
            {
                _allScenes.ForEach(scene => SceneMetaAsset.Validate(scene));
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException(ex.Message + editorMessage);
            }

            if (_allScenes.Distinct().Count() != _allScenes.Count)
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

            EditorSceneManager.OpenScene(_coreScene.SceneReference.Path);
        }
#endif
    }
}