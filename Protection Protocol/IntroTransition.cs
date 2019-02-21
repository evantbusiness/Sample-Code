using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroTransition : MonoBehaviour
{	
	void Update ()
    {
        if (this.GetComponent<VideoPlayer>().time >= this.GetComponent<VideoPlayer>().clip.length - 0.1f)
            SceneManager.LoadScene("Main Menu");
	}
}
