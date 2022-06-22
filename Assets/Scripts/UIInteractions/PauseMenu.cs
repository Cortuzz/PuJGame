using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UIInteractions
{
    public class PauseMenu : MonoBehaviour
    {
        public GameObject pauseMenuUI;
        public TMP_Text[] menuText;

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            if (World.isGamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        // Hard coding timescale can lead to errors if the game time will speed up or slow down 
        public void Resume()
        {
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            World.isGamePaused = false;

            foreach (var text in menuText)
            {
                text.color = MenuButton.baseColor;
            }
        }

        public void Pause()
        {
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            World.isGamePaused = true;
        }

        public void LoadMenu()
        {
            Debug.Log("Loading menu");
            SceneManager.LoadScene("MainMenu");
        }

        public void QuitGame()
        {
            Debug.Log("Quiting the game");
            Application.Quit();
        }
    }
}