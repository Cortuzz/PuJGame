using UnityEngine;
using UnityEngine.UI;

namespace UIInteractions.Settings
{
    public class SwitchButtonsSelect: MonoBehaviour
    {
        public Button[] buttons;

        private readonly Color _selected = new Color(0.1981132f, 0.1981132f, 0.1981132f);
        private readonly Color _notSelected = new Color(1, 1, 1);

        public void MakeSelected(Button currButton)
        {
            foreach (var button in buttons)
            {
                button.image.color = _notSelected;
            }

            currButton.image.color = _selected;
        }
    }
}