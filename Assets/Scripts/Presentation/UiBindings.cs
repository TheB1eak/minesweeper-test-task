using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Minesweeper.Presentation
{
    internal static class UiBindings
    {
        public static void SetPanelActive(GameObject panel, bool isActive)
        {
            if (panel != null)
                panel.SetActive(isActive);
        }

        public static void BindButton(Button button, UnityAction handler)
        {
            if (button != null)
                button.onClick.AddListener(handler);
        }

        public static void UnbindButton(Button button, UnityAction handler)
        {
            if (button != null)
                button.onClick.RemoveListener(handler);
        }
    }
}
