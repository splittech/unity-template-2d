using Game.Core;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.MainMenu
{
    public class ExitButton : MonoBehaviour
    {
        [SerializeField] private Button _button;

        private ApplicationService _applicationService;

        [Inject]
        public void Construct(ApplicationService applicationService)
        {
            _applicationService = applicationService;

            _button.GetComponent<Button>();
            _button.OnClickAsObservable()
                .Subscribe(_ => OnButtonClicked())
                .AddTo(this);
        }

        private void OnButtonClicked()
        {
            _applicationService.ExitGame();
        }
    }
}
