using System;
using System.Collections.Generic;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenu
{
    public class MenuPanelController : MonoBehaviour
    {
        [SerializeField] private List<MenuPanelSettings> _menuPanels;

        private MenuPanel _activePanel;

        private void Start()
        {
            foreach (var settings in _menuPanels)
            {
                MenuPanel panel = settings.Panel;
                foreach (var button in settings.Buttons)
                {
                    button.OnClickAsObservable()
                        .Subscribe(_ => SwitchPanel(panel))
                        .AddTo(this);
                }
            }
        }

        private void SwitchPanel(MenuPanel panel)
        {
            if (_activePanel == panel)
                return;

            _activePanel = panel;

            _menuPanels.ForEach(settings => settings.Panel.Hide());
            panel.Show();
        }

        [Serializable]
        private class MenuPanelSettings
        {
            public MenuPanel Panel;
            public List<Button> Buttons;
        }
    }
}