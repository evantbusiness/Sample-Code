using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menuScript : MonoBehaviour {

    void Start()
    {
        Screen.SetResolution(640, 960, false);
    }

    public void loadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void loadLevel()
    {
        SceneManager.LoadScene(1);
    }

    public void loadInstructions()
    {
        SceneManager.LoadScene(2);
    }

    public void loadCredits()
    {
        SceneManager.LoadScene(3);
    }
}
