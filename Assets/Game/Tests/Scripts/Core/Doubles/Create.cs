using System;
using System.Collections.Generic;
using Eflatun.SceneReference;
using UnityEngine;

namespace Game.Core.Tests.Doubles
{
    public class Create
    {
        public static SceneMetaAsset SceneMetaAsset(
            SceneReference sceneReference = null,
            bool initial = false,
            bool persistent = false)
        {
            SceneMetaAsset sceneMetaAsset = ScriptableObject.CreateInstance<SceneMetaAsset>();

            sceneMetaAsset.name = "";
            sceneMetaAsset.SceneReference = sceneReference ?? new SceneReference();
            sceneMetaAsset.Initial = initial;
            sceneMetaAsset.Persistent = persistent;

            return sceneMetaAsset;
        }

        public static SceneServiceConfig SceneServiceConfig(
            SceneMetaAsset coreScene = null,
            List<SceneMetaAsset> otherScenes = null)
        {
            SceneServiceConfig config = ScriptableObject.CreateInstance<SceneServiceConfig>();

            config.CoreScene = coreScene == null ? Create.SceneMetaAsset() : coreScene;
            config.OtherScenes = otherScenes ?? new List<SceneMetaAsset>();

            return config;
        }

        public static IProgress<float> Progress()
        {
            return new ImmediateProgress<float>(_ => { });
        }
    }
}