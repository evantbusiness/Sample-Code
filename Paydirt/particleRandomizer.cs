using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particleRandomizer : MonoBehaviour {

    public Material[] particleMat;
    public Renderer rend;
    public int i;

	// Use this for initialization
	void Start () {
        rend = GetComponent<Renderer>();
        rend.enabled = true;
    }
	
	// Update is called once per frame
	void Update () {

        i = Random.Range(0, 4);

        rend.sharedMaterial = particleMat[i];
    }
}
