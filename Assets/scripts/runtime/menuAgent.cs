using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class menuAgent : MonoBehaviour
{
    public Canvas pauseMenu;

    public levelGeneratorScript levelGen;
    bool isPaused = false;

    private void Start()
    {
        levelGen.generateLevel();

        Resume();
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    void Pause()
    {
        pauseMenu.enabled = true;
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        pauseMenu.enabled = false;
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void switchToMainMenu()
    {
        SceneManager.LoadScene(0);        
    }

    public void exitGame()
    {
        Application.Quit();
    }
}
