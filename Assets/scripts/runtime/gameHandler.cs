using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class gameHandler : MonoBehaviour
{
    public static string gameSeed;
    public static int gameMagnitude;
    public static int numMonsters;


    public Canvas entryCanvas;
    public Canvas difficultySelectionCanvas;
    public Canvas customGameConfiguration;
    public Canvas settingsMenu;

    public Dropdown resolutionSettings;

    public List<Text> customGameSettings;

    [HideInInspector] public difficulty diff;

    Resolution[] resolutions;

    public enum difficulty
    {
        easy,
        normal,
        hard,
        custom
    }

    private void Start()
    {
        switchMenuToEntry();

        resolutions = Screen.resolutions;
        resolutionSettings.ClearOptions();

        List<string> options = new List<string>();

        int currentResIndex = 0;
        for (int i =0; i< resolutions.Length; i++)
        {
            string option = resolutions[i].width + " X " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResIndex = i;
            }
        }

        resolutionSettings.AddOptions(options);
        resolutionSettings.value = currentResIndex;
        resolutionSettings.RefreshShownValue();
    }

    public void playThegame()
    {
        switch (diff)
        {
            case difficulty.easy:
                gameSeed = Random.Range(int.MinValue, int.MaxValue).ToString();
                gameMagnitude = 50;
                numMonsters = 2;
                break;
            case difficulty.normal:
                gameSeed = Random.Range(int.MinValue, int.MaxValue).ToString();
                gameMagnitude = 100;
                numMonsters = 4;
                break;
            case difficulty.hard:
                gameSeed = Random.Range(int.MinValue, int.MaxValue).ToString();
                gameMagnitude = 200;
                numMonsters = 16;
                break;
            case difficulty.custom:
                gameSeed = customGameSettings[0].text;

                gameMagnitude = int.Parse(customGameSettings[1].text);
                if (gameMagnitude == 0) { gameMagnitude = 10; }
                if (gameMagnitude < 0 ) { gameMagnitude *= -1; }

                numMonsters = int.Parse(customGameSettings[2].text);
                if (numMonsters < 0) { numMonsters *= -1; }
                break;
            default:
                break;

        }
        SceneManager.LoadScene(1);

    }

    public void quitGame()
    {
        Application.Quit();
    }

    public void setGameEasy() { diff = difficulty.easy; }
    public void setGameNormal() { diff = difficulty.normal; }
    public void setGameHard() { diff = difficulty.hard; }
    public void setGameCustom() { diff = difficulty.custom; switchMenuToCustomConfig(); }

    public void switchMenuToEntry()
    {
        entryCanvas.enabled = true;
        difficultySelectionCanvas.enabled = false;
        customGameConfiguration.enabled = false;
        settingsMenu.enabled = false;
    }
    public void switchMenuToDiffSelection()
    {
        entryCanvas.enabled = false;
        difficultySelectionCanvas.enabled = true;
        customGameConfiguration.enabled = false;
        settingsMenu.enabled = false;
    }
    public void switchMenuToCustomConfig()
    {
        entryCanvas.enabled = false;
        difficultySelectionCanvas.enabled = false;
        customGameConfiguration.enabled = true;
        settingsMenu.enabled = false;
    }
    public void switchMenuToSettings()
    {
        entryCanvas.enabled = false;
        difficultySelectionCanvas.enabled = false;
        customGameConfiguration.enabled = false;
        settingsMenu.enabled = true;
    }

    public void setQuality(int quality)
    {
        QualitySettings.SetQualityLevel(quality);
    }

    public void setFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void setResolution(int resIndex)
    {
        Screen.SetResolution(
            resolutions[resIndex].width, 
            resolutions[resIndex].height, 
            Screen.fullScreen);
    }
}
