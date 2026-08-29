using Cysharp.Threading.Tasks;
using NSubstitute;

namespace Game.Core.Tests.Doubles
{
    public class Setup
    {
        public static SceneService SceneService(SceneServiceConfig config = null)
        {
            if (config == null)
                config = Create.SceneServiceConfig();

            ILoggingService loggingService = Substitute.For<ILoggingService>();
            loggingService.GetLogger(default).ReturnsForAnyArgs(new EmptyLogger(null));

            ISceneLoader sceneLoader = Substitute.For<ISceneLoader>();
            sceneLoader.LoadSceneAsync(default, default, default).ReturnsForAnyArgs(UniTask.CompletedTask);
            sceneLoader.UnloadSceneAsync(default, default, default).ReturnsForAnyArgs(UniTask.CompletedTask);

            return new SceneService(config, loggingService, sceneLoader);
        }
    }
}