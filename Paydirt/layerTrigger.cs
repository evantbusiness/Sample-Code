using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class layerTrigger : MonoBehaviour {

    public GameObject audioController;
    public AudioMixerSnapshot firstLayer;
    public AudioMixerSnapshot secondLayer;
    public AudioMixerSnapshot thirdLayer;
    public int currentLayer;

    // Use this for initialization
    void Start () {
        currentLayer = 1;
    }

    private void OnTriggerEnter2D(Collider2D collide)
    {
        if (collide.gameObject.tag == "layerTransition")
        {
            mixLevels mixer = audioController.GetComponent<mixLevels>();
            currentLayer += 1;

            if (currentLayer == 2)
            {
                mixer.layerLevels(secondLayer);
            }

            if (currentLayer == 3)
            {
                mixer.layerLevels(thirdLayer);
            }
        }
    }
}
