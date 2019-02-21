using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class depthCounter : MonoBehaviour
{

    public GameObject depthUI;

    // Update is called once per frame
    void Update()
    {
        depthCalc();
    }

    private void depthCalc()
    {
        Text depthNum = depthUI.GetComponent<Text>();
        int depthValue = (int) (Time.time - this.GetComponent<characterControls>().timePassed);
        depthNum.text = depthValue.ToString();
    }
}

