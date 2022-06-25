using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIInteractions.Settings
{
    public class SwitchButton : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler
    {
        private bool _isMouseDown;
        private TMP_Text _text;
        
        private void Awake()
        {
            _text = gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _text.color = MenuButton.HoverColor;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isMouseDown = true;
            _text.color = MenuButton.HoverColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isMouseDown) return;
            _text.color = MenuButton.BaseColor;
        }
    }
}