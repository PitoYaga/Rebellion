using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public bool isPaused;
    private Player _playerCS;

    void Start()
    {
        pauseMenu.SetActive(false);
        _playerCS = FindObjectOfType<Player>();
    }
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Paused();
            }
        } 
    }

    public void Paused()
    {
        Cursor.visible = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        _playerCS.enabled = false;
        isPaused = true;
    }

    public void Resume()
    {
        _playerCS.enabled = true;
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.visible = false;
        pauseMenu.SetActive(false);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene("MainMenu");
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene("Level1");
    }

    public void Quit()
    {
        Application.Quit();
    }
}   
