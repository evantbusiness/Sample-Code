using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menuMusicHandler : MonoBehaviour {

    private static menuMusicHandler instance = null;
    public static menuMusicHandler Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        } else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
	
	void Update () {
		if(SceneManager.GetActiveScene().name == "Level")
        {
            Destroy(this.gameObject);
        }
	}
}
