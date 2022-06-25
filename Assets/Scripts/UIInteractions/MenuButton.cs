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
        public static Color BaseColor = new(0.1960784f, 0.1960784f, 0.1960784f);
        public static Color HoverColor = new(0.9339623f, 0.9339623f, 0.9339623f);

        private bool _isMouseDown;

        public void OnPointerEnter(PointerEventData eventData)
        {
            theText.color = HoverColor;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isMouseDown = false;
            theText.color = BaseColor;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isMouseDown = true;
            theText.color = HoverColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isMouseDown) return;
            theText.color = BaseColor;
        }
    }
}