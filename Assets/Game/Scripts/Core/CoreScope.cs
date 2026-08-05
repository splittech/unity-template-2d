using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Core
{
    public class CoreScope : LifetimeScope
    {
        [Header("Loading Screen")]
        [SerializeField] private LoadingScreen _loadingScreen;

        [Header("Scene Service")]
        [SerializeField] private SceneServiceConfig _sceneServiceConfig;

        [Header("Sound Service")]
        [SerializeField] private AudioServiceConfig _audioServiceConfig;
        [SerializeField] private MusicPlayer _musicPlayer;
        [SerializeField] private SoundPlayer _soundPlayer;

        protected override void Configure(IContainerBuilder builder)
        {
            // Bootstrap
            builder.RegisterEntryPoint<CoreBootstrap>();

            // Loading Screen
            builder.RegisterComponent(_loadingScreen);

            // Scene Service
            builder.Register<SceneService>(Lifetime.Singleton);
            builder.RegisterInstance(_sceneServiceConfig);
            builder.Register<ISceneLoader, SceneLoader>(Lifetime.Singleton);

            // Audio Service
            builder.Register<AudioService>(Lifetime.Singleton);
            builder.RegisterInstance(_audioServiceConfig);
            builder.RegisterInstance(_musicPlayer);
            builder.RegisterInstance(_soundPlayer);

            // Addressables Service
            builder.Register<AddressablesService>(Lifetime.Singleton);
            builder.Register<IAddressablesLoader, AddressablesLoader>(Lifetime.Singleton);
        }

    }
}
