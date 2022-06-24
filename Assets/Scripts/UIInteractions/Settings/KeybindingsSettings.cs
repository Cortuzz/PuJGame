using System;
using System.Collections.Generic;
using System.Linq;
using Configs;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace UIInteractions.Settings
{
    public class KeybindingsSettings : MonoBehaviour
    {
        private KeyCode _pressedKey;
        [CanBeNull] private string _changeableKeyName;
        private TMP_Text _keyNameText;

        private void OnGUI()
        {
            var e = Event.current;

            if (!e.isKey || _changeableKeyName == null) return;

            _pressedKey = e.keyCode;

            if (KeyboardBindings.CheckCollisions(_changeableKeyName, (short)_pressedKey))
            {
                _keyNameText.text = KeyToString(KeyboardBindings.Config[_changeableKeyName]);
            }

            KeyboardBindings.Config[_changeableKeyName] = _pressedKey;

            _keyNameText.text = KeyToString(_pressedKey);

            _changeableKeyName = null;
            Debug.Log(KeyboardBindings.Config.Aggregate("",
                (current, kv) => current + (kv.Key + " " + kv.Value + " ")));
        }

        private static string KeyToString(KeyCode pressedKey)
        {
            return (short)pressedKey is >= 48 and <= 58
                ? pressedKey.ToString().Replace("Alpha", "")
                : pressedKey.ToString();
        }

        private void SetStartBindings()
        {
            
        }
        
        public void OnKbChangeClick(GameObject clickedButton)
        {
            var cbTransform = clickedButton.transform;

            _changeableKeyName = cbTransform.tag;
            _keyNameText = cbTransform.GetChild(1).gameObject.GetComponent<TMP_Text>();
            _keyNameText.text = "...";
        }
    }
}