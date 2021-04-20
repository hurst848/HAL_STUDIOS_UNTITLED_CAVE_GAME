using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gameHandler : MonoBehaviour
{
    public static string gameSeed;
    public static int gameMagnitude;
    public static int numMonsters;


    [HideInInspector] public difficulty diff;

    public enum difficulty
    {
        easy,
        normal,
        hard,
        custom
    }
    


    void playThegame()
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
                // do nothing as values are already assigned
                break;
            default:
                break;

        }
        SceneManager.LoadScene(1);

    }
}
