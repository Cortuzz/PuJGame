using UnityEngine;
using UnityEngine.SceneManagement;

namespace UIInteractions
{
    public class MainMenu : MonoBehaviour
    {
        public GameObject mainMenu;
        public GameObject settingsMenuUI;

        public void PlayGame()
        {
            SceneManager.LoadScene("WorldGen");
        }
    
        public void LoadGame()
        {
            WorldSaving.WorldSavingController.LoadFromFile();
        }
    
        public void OpenSettings()
        {
            mainMenu.SetActive(false);
            settingsMenuUI.SetActive(true);
            PrevCurrStates.SetStates(mainMenu, settingsMenuUI);
        }
    
        public void QuitGame()
        {
            Debug.Log("Quiting the game");
            Application.Quit();
        }
    }
}
