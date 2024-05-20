using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{

    public void LoadGame()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadShop()
    {
        SceneManager.LoadScene(2);
    }    

    public void Restart()
    {
        SavingSystem.DeleteGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    public bool ChangeSoundState()
    {
        bool currentState = PlayerPrefs.GetInt("SoundState", 1) == 1 ? true : false;
        PlayerPrefs.SetInt("SoundState", currentState ? 0 : 1);

        return !currentState;
    }
}
