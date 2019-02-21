using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionScript : MonoBehaviour
{
    [SerializeField]
    private GameObject controller;

    [SerializeField]
    private GameObject keyboard;

    //Initialization
    private void Awake ()
    {
        controller.SetActive(true);
        keyboard.SetActive(false);
	}
	
	public void DisplayController()
    {
        keyboard.SetActive(false);
        controller.SetActive(true);
    }

    public void DisplayKeyboard()
    {
        controller.SetActive(false);
        keyboard.SetActive(true);
    }
}
