using System;
using System.Linq;

namespace Game.Core
{
    public static class SceneServiceValidator
    {
        public static void ValidateSceneMetaAsset(SceneMetaAsset sceneAsset)
        {
            if (sceneAsset == null)
            {
                throw new InvalidOperationException(
                    $"SceneMetaAsset 'null': Scene asset can't be null.");
            }

            if (sceneAsset.SceneReference == null)
            {
                throw new InvalidOperationException(
                    $"SceneMetaAsset '{sceneAsset.name}': Scene asset has no scene reference.");
            }
        }

        public static void ValidateSceneList(SceneServiceConfig _config)
        {
            _config.AllScenes.ForEach(scene => ValidateSceneMetaAsset(scene));

            if (_config.AllScenes.Distinct().Count() != _config.AllScenes.Count)
                throw new InvalidOperationException("Scene config contains duplicates.");
        }
    }
}