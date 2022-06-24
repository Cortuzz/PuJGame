using UnityEngine;

namespace UIInteractions.Settings
{
    public class SwitchTabs: MonoBehaviour
    {
        public GameObject keybindingsWindow;
        public GameObject volumeWindow;
        public GameObject graphicsWindow;

        public void OpenGraphics()
        {
            volumeWindow.SetActive(false);
            keybindingsWindow.SetActive(false);
            
            graphicsWindow.SetActive(true);
        }
        
        public void OpenVolume()
        {
            graphicsWindow.SetActive(false);
            keybindingsWindow.SetActive(false);
            
            volumeWindow.SetActive(true);
        }
        
        public void OpenKeybindings()
        {
            volumeWindow.SetActive(false);
            graphicsWindow.SetActive(false);
            
            keybindingsWindow.SetActive(true);
        }
    }
}