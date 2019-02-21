using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleExpiration : MonoBehaviour {

    private ParticleSystem parts;
    private float totalDuration;

	// Use this for initialization
	void Awake ()
    {
        parts = this.GetComponent<ParticleSystem>();

        totalDuration = parts.main.duration + parts.main.startLifetime.constant;
	}
	
	// Update is called once per frame
	void Update ()
    {
        Destroy(this.gameObject, totalDuration);
	}
}
