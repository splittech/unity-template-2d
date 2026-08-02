using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Game.Core.Tests.Editor
{
    internal sealed class TestSceneServiceConfig : SceneServiceConfig
    {
        private readonly List<SceneMetaAsset> scenes = new();

        public void Initialize(SceneMetaAsset coreScene, params SceneMetaAsset[] otherScenes)
        {
            SceneServiceTestData.SetCoreScene(this, coreScene);
            scenes.Clear();
            scenes.AddRange(otherScenes);
        }

        protected override void FillAllSceneList(List<SceneMetaAsset> allScenes)
        {
            allScenes.AddRange(scenes);
        }
    }

    internal static class SceneServiceTestData
    {
        private const BindingFlags InstanceField =
            BindingFlags.Instance | BindingFlags.NonPublic;

        private static readonly FieldInfo SceneReferenceField =
            GetRequiredField(typeof(SceneMetaAsset), "_sceneReference");
        private static readonly FieldInfo InitialField =
            GetRequiredField(typeof(SceneMetaAsset), "_initial");
        private static readonly FieldInfo PersistentField =
            GetRequiredField(typeof(SceneMetaAsset), "_persistent");
        private static readonly FieldInfo CoreSceneField =
            GetRequiredField(typeof(SceneServiceConfig), "_coreScene");

        public static SceneMetaAsset CreateScene(bool initial = false, bool persistent = false)
        {
            SceneMetaAsset scene = ScriptableObject.CreateInstance<SceneMetaAsset>();

            // The mock loader never resolves the path; validation only requires
            // the reference object itself to exist.
            SceneReferenceField.SetValue(
                scene,
                Activator.CreateInstance(SceneReferenceField.FieldType));
            InitialField.SetValue(scene, initial);
            PersistentField.SetValue(scene, persistent);
            return scene;
        }

        public static TestSceneServiceConfig CreateConfig(
            SceneMetaAsset coreScene,
            params SceneMetaAsset[] otherScenes)
        {
            TestSceneServiceConfig config =
                ScriptableObject.CreateInstance<TestSceneServiceConfig>();
            config.Initialize(coreScene, otherScenes);
            return config;
        }

        public static void SetCoreScene(
            SceneServiceConfig config,
            SceneMetaAsset coreScene)
        {
            CoreSceneField.SetValue(config, coreScene);
        }

        private static FieldInfo GetRequiredField(Type type, string name)
        {
            return type.GetField(name, InstanceField) ??
                   throw new MissingFieldException(type.FullName, name);
        }
    }
}
