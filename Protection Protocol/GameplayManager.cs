using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{
    [SerializeField]
    private int levelTimeLimit;

    [SerializeField]
    private string musicTrackName;

    [SerializeField]
    private GameObject pauseScreen;

    [SerializeField]
    private GameObject camera;

    private int nextLevelIndex;

    private SceneTransition sceneTransition;
    private bool isTransitioning;
    private float transitionTimer;

    private float timer;
    private float stopTime;

    private bool isPaused;

    public float Timer
    {
        get { return timer; }
        set { timer = value; }
    }

    private void Start()
    {
        Cursor.visible = false;

        timer = levelTimeLimit;

        sceneTransition = GameObject.Find("GameplayManager").GetComponent<SceneTransition>();
        transitionTimer = 0;

        isPaused = false;

        SoundManager.Instance.PlayMusic(musicTrackName);
    }

    private void Update()
    {
        //Level time limit
        if (timer > 0)
            timer -= Time.deltaTime;
        else
            GameOver();

        //Time stop effect for impacts
        if (stopTime > 0)
            stopTime -= Time.unscaledDeltaTime;
        else if(stopTime <= 0 && !isPaused)
            Time.timeScale = 1;

        if(Input.GetButtonDown("Pause") && !isPaused)
        {
            isPaused = true;
            pauseScreen.SetActive(true);
            Cursor.visible = true;
            Time.timeScale = 0;
        }
        else if (Input.GetButtonDown("Pause") && isPaused)
        {
            isPaused = false;
            pauseScreen.SetActive(false);
            Cursor.visible = false;
            Time.timeScale = 1;
        }

        if(isTransitioning)
        {
            sceneTransition.OutroBlackScreenPlay();
            transitionTimer += Time.deltaTime;
            if (transitionTimer >= 1)
            {
                SceneManager.LoadScene(nextLevelIndex);
            }
        }
    }

    public void NextLevel()
    {
        nextLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (SceneManager.sceneCountInBuildSettings > nextLevelIndex)
            isTransitioning = true;
        else
            SceneManager.LoadScene(0);
    }

    public void GameOver()
    {
        SceneManager.LoadScene("GameOver");
    }

    //Return to main menu from pause screen
    public void MainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Main Menu");
    }

    //Resume game from pause screen continue button
    public void Continue()
    {
        isPaused = false;
        pauseScreen.SetActive(false);
        Cursor.visible = false;
        Time.timeScale = 1;
    }

    //Slowdown and screenshake
    public void WorldImpactEffect(float stopTime, float camShakeTime, float camShakeIntensity)
    {
        this.stopTime = stopTime;
        Time.timeScale = 0.2f;
        camera.GetComponent<CameraScript>().ShakeCam(camShakeTime, camShakeIntensity);
    }
}
