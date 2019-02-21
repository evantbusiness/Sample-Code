using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collectableSpawner : MonoBehaviour {

    public int collectableValue;
    public int IDNum;
    public float speed;
    public Sprite[] items;
    public GameObject player;
	
	// Update is called once per frame
	void Update () {

        if (IDNum <= player.GetComponent<layerTrigger>().currentLayer)
        {
            transform.position += new Vector3(0, speed, 0);
        }
        
        //transform.position += new Vector3(0, speed, 0);   
        
        if(transform.position.y > spawnBounds().y)
        {
            spawnCalc();
        }

    }

    Vector3 spawnBounds()
    {
        Vector3 boundingVars = new Vector3(0, Random.Range(1250, 2500), 0);
        Vector3 boundingArea = Camera.main.ScreenToWorldPoint(boundingVars);

        return boundingArea;
    }

    public void spawnCalc()
    {
        int size = items.Length;

        gameObject.GetComponent<SpriteRenderer>().sprite = items[Random.Range(0, size)];

        Vector3 spawnPos = new Vector3(Random.Range(0, 640), Random.Range(-50, -1500), 11);
        transform.position = Camera.main.ScreenToWorldPoint(spawnPos);
    }
}
