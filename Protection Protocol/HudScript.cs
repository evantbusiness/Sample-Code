using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudScript : MonoBehaviour {

    [SerializeField]
    private Text timeDisplay;

    [SerializeField]
    private Slider healthBar;

    [SerializeField]
    private Image fill;

    private GameObject player;
    private GameObject gameplayManager;

    private Color maxHealthColor;
    private Color minHealthColor;

    private int minutes;
    private int seconds;

    private bool lowTime;

    private void Awake()
    {
        player = GameObject.Find("PlayerFinal");
        gameplayManager = GameObject.Find("GameplayManager");

        maxHealthColor = Color.green;
        minHealthColor = Color.red;

        lowTime = false;
    }

    private void Update ()
    {
        CountDown();
        HealthUpdate();
    }

    private void CountDown()
    {
        minutes = (int)gameplayManager.GetComponent<GameplayManager>().Timer / 60;
        seconds = (int)gameplayManager.GetComponent<GameplayManager>().Timer % 60;

        if (minutes == 0 && !lowTime)
        {
            timeDisplay.color = Color.red;
            lowTime = true;
        }

        if (seconds > 9)
            timeDisplay.text = minutes + ":" + seconds;
        else
            timeDisplay.text = minutes + ":0" + seconds;
    }

    private void HealthUpdate()
    {
        healthBar.value = player.GetComponent<PlayerController>().Health;
        fill.color = Color.Lerp(minHealthColor, maxHealthColor, healthBar.value/100);
    }
}
