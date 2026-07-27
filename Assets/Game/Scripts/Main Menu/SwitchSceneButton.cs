using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.MainMenu
{
    public class SwitchSceneButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private SceneMetaAsset sceneToSwitch;
        [SerializeField] private string loadingScreenDescription;

        private LoadingScreen loadingScreen;
        private SceneService sceneService;

        [Inject]
        public void Construct(LoadingScreen loadingScreen, SceneService sceneService)
        {
            this.loadingScreen = loadingScreen;
            this.sceneService = sceneService;

            button.OnClickAsObservable()
                .Subscribe(_ => SwitchToMainMenuSceneAsync().Forget())
                .AddTo(this);
        }

        private async UniTask SwitchToMainMenuSceneAsync(CancellationToken ct = default)
        {
            loadingScreen.SetProgressDescription(loadingScreenDescription);
            await loadingScreen.Show(ct);

            await sceneService.SwitchScene(sceneToSwitch, loadingScreen.Progress, ct);

            await loadingScreen.Hide(ct);
        }
    }
}
