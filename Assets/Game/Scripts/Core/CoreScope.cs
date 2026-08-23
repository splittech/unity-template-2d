using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Core
{
    public class CoreScope : LifetimeScope
    {
        [Header("Loading Screen")]
        [SerializeField] private LoadingScreen _loadingScreen;

        [Header("Logging Service")]
        [SerializeField] private LoggingServiceConfig _loggingServiceConfig;

        [Header("Scene Service")]
        [SerializeField] private SceneServiceConfig _sceneServiceConfig;

        [Header("Sound Service")]
        [SerializeField] private AudioServiceConfig _audioServiceConfig;
        [SerializeField] private MusicPlayer _musicPlayer;
        [SerializeField] private SoundPlayer _soundPlayer;

        [Header("Addressables Service")]
        [SerializeField] private AddressablesServiceConfig _addressablesServiceConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            // Bootstrap
            builder.RegisterEntryPoint<CoreBootstrap>();

            // Loading Screen
            builder.RegisterComponent(_loadingScreen);

            // Application Service
            builder.Register<ApplicationService>(Lifetime.Singleton);

            // Logging Service
            builder.Register<ILoggingService, LoggingService>(Lifetime.Singleton);
            builder.Register<LoggerFactory>(Lifetime.Singleton);
            builder.RegisterInstance(_loggingServiceConfig);

            // Scene Service
            builder.Register<SceneService>(Lifetime.Singleton);
            builder.Register<ISceneLoader, SceneLoader>(Lifetime.Singleton);
            builder.RegisterInstance(_sceneServiceConfig);

            // Audio Service
            builder.Register<AudioService>(Lifetime.Singleton);
            builder.Register<AudioSettings>(Lifetime.Singleton);
            builder.RegisterInstance(_audioServiceConfig);
            builder.RegisterComponent(_musicPlayer);
            builder.RegisterComponent(_soundPlayer);

            // Addressables Service
            builder.Register<AddressablesService>(Lifetime.Singleton);
            builder.Register<IAddressablesLoader, AddressablesLoader>(Lifetime.Singleton);
            builder.RegisterInstance(_addressablesServiceConfig);
        }

    }
}
