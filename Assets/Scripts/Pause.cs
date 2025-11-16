using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject PauseButton;
    public bool PauseGame;
    public GameObject gamePauseMenu;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PauseGame)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }
    public void Resume()
    {
        gamePauseMenu.SetActive(false);
        Time.timeScale = 1f;
        PauseGame = false;
        PauseButton.SetActive(true);
    }
    public void Pause()
    {
        gamePauseMenu.SetActive(true);
        Time.timeScale = 0f;
        PauseGame = true;
        PauseButton.SetActive(false);
    }
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
