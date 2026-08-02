using System.Collections.Generic;
using UnityEngine;

namespace Game.Core
{
    [CreateAssetMenu(fileName = "Scene Service Config", menuName = "Game/Configs/Scene Service/Scene Service Config", order = 0)]
    public class BasicSceneServiceConfig : SceneServiceConfig
    {
        [SerializeField] private SceneMetaAsset _mainMenuScene;
        [SerializeField] private SceneMetaAsset _gameplayScene;

        public SceneMetaAsset MainMenuScene => _mainMenuScene;
        public SceneMetaAsset GameplayScene => _gameplayScene;

        protected override void FillAllSceneList(List<SceneMetaAsset> allScenes)
        {
            allScenes.Add(_mainMenuScene);
            allScenes.Add(_gameplayScene);
        }
    }
}
