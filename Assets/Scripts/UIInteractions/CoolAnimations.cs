using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIInteractions
{
    public class CoolAnimations : MonoBehaviour
    {
        public GameObject animationsContainer;

        private bool _isActive = true;
        
        public void SwitchAnimation(Button button)
        {
            var buttonText = button.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
            animationsContainer.SetActive(!_isActive);
            _isActive = !_isActive;
            buttonText.text = _isActive ? "Turn Off" : "Turn On";
        }
    }
}
