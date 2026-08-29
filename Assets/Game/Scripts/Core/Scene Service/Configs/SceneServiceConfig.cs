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
    [CreateAssetMenu(menuName = "Game/Configs/Scene Service Config", order = 0)]
    public class SceneServiceConfig : ScriptableObject
    {
        public SceneMetaAsset CoreScene;
        public List<SceneMetaAsset> OtherScenes;

        public List<SceneMetaAsset> AllScenes =>
            OtherScenes.Append(CoreScene).ToList();

        public List<SceneMetaAsset> NonPersistentScenes =>
            OtherScenes.Where(scene => !scene.Persistent).ToList();

        public List<SceneMetaAsset> InitialScenes =>
            OtherScenes.Where(scene => scene.Initial).ToList();

#if UNITY_EDITOR

        [BoxGroup("Open Only Core Scene")]
        [Button]
        private void OpenOnlyCoreScene()
        {
            AllScenes.ForEach(sceneAsset =>
            {
                Scene scene = SceneManager.GetSceneByName(sceneAsset.SceneReference.Name);
                EditorSceneManager.CloseScene(scene, true);
            });

            EditorSceneManager.OpenScene(CoreScene.SceneReference.Path);
        }

#endif

    }
}
