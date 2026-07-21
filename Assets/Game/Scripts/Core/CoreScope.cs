using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Core
{
    public class CoreScope : LifetimeScope
    {
        [Header("Loading Screen")]
        [SerializeField] private LoadingScreen loadingScreen;

        [Header("Scene Service")]
        [SerializeField] private SceneServiceConfig sceneServiceConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            // Bootstrap
            builder.RegisterEntryPoint<CoreBootstrap>();

            // Loading Screen
            builder.RegisterComponent(loadingScreen);

            // Scene Service
            builder.RegisterInstance(sceneServiceConfig);
            builder.Register<SceneLoader, UnitySceneLoader>(Lifetime.Singleton);
            builder.Register<SceneService>(Lifetime.Singleton);
        }

    }
}
