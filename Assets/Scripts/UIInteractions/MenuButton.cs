using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace UIInteractions
{
    public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerUpHandler, IPointerDownHandler,
        IPointerExitHandler
    {
        public TMP_Text theText;
        public static Color baseColor = new(0, 0, 0);
        public static Color hoverColor = new(234f, 234f, 234f);

        private bool _isMouseDown;

        public void OnPointerEnter(PointerEventData eventData)
        {
            theText.color = hoverColor;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isMouseDown = false;
            theText.color = baseColor;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isMouseDown = true;
            theText.color = hoverColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isMouseDown) return;
            theText.color = baseColor;
        }
    }
}