using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class menuAgent : MonoBehaviour
{
    public Canvas pauseMenu;

    public levelGeneratorScript levelGen;


    public Canvas lossMenu;
    public Canvas winMenu;
    public Canvas hud;
    public Canvas loadingScreen;

    public Slider progressBar;

    bool isPaused = false;
    bool loadingLevel = false;
    private void Start()
    {
        
        lossMenu.enabled = false;
        winMenu.enabled = false;
        Resume();
    }


    // Update is called once per frame
    void Update()
    {
        if (loadingLevel)
        {
            float currentProgress =
                GameObject.FindGameObjectWithTag("lvlgen").GetComponent<levelGeneratorScript>().generatedlevel.Count /
                (float)GameObject.FindGameObjectWithTag("lvlgen").GetComponent<levelGeneratorScript>().magnitude;
            progressBar.value = currentProgress;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && health.currentHP > 0)
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
        if (health.currentHP <= 0)
        {
            lossMenu.enabled = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
        if (GameObject.FindGameObjectWithTag("Player").GetComponent<cameraController>().won)
        {
            winMenu.enabled = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    public void loading()
    {
        loadingLevel = true;
        hud.enabled = false;
        loadingScreen.enabled = true;
    }
    public void doneLoading()
    {
        hud.enabled = true;
        loadingScreen.enabled = false;
    }


    void Pause()
    {
        loadingLevel = true;
        pauseMenu.enabled = true;
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void Resume()
    {
        pauseMenu.enabled = false;
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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
