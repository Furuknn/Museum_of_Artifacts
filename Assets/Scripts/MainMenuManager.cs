using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    public void PlayButton()
    {
        SceneManager.LoadScene("LIVE_SCENE");
    }

    public void SettingsButton()
    {

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
