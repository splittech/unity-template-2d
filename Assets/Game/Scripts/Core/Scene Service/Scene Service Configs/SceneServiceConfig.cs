using System;
using System.Collections.Generic;
using System.Linq;
using Alchemy.Inspector;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR

using UnityEditor.SceneManagement;

#endif

namespace Game.Core
{
    public abstract class SceneServiceConfig : ScriptableObject
    {
        [Header("Debug")]
        [SerializeField] private bool _enableLogger;

        [Header("Core Scene")]
        [SerializeField] private SceneMetaAsset _coreScene;

        private readonly List<SceneMetaAsset> _allScenes = new();

        public SceneMetaAsset CoreScene => _coreScene;
        public List<SceneMetaAsset> AllScenes => _allScenes.ToList();
        public bool EnableLogger => _enableLogger;

        public void RecreateAllScenesList()
        {
            _allScenes.Clear();
            _allScenes.Add(_coreScene);
            FillAllSceneList(_allScenes);
            ValidateAllScenesList();
        }

        public List<SceneMetaAsset> GetAllScenes()
        {
            return _allScenes.ToList();
        }

        public List<SceneMetaAsset> GetAllScenesExceptCore()
        {
            return _allScenes.Where(scene => scene != _coreScene).ToList();
        }

        public List<SceneMetaAsset> GetAllNonPersistentScenes()
        {
            return _allScenes.Where(scene => !scene.Persistent && scene != _coreScene).ToList();
        }

        public List<SceneMetaAsset> GetAllInitialScenes()
        {
            return _allScenes.Where(scene => scene.Initial && scene != _coreScene).ToList();
        }

        protected abstract void FillAllSceneList(List<SceneMetaAsset> allScenes);

        private void ValidateAllScenesList()
        {
            _allScenes.ForEach(scene => SceneMetaAsset.Validate(scene));

            if (_allScenes.Distinct().Count() != _allScenes.Count)
                throw new InvalidOperationException("Scene list contains duplicates.");

            GameLogger.Log($"SceneServiceConfig '{name}': Scene list validation is successful.");
        }

#if UNITY_EDITOR
        [BoxGroup("Open Only Core Scene")]
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
