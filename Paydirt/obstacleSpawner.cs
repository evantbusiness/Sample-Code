using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obstacleSpawner : MonoBehaviour {

    public float speed;
    public GameObject[] obstacles;
    public GameObject currObstacle;
    private SpriteRenderer spriteRender;
    public PolygonCollider2D collide; 


    // Use this for initialization
    void Start () {
        currObstacle = obstacles[Random.Range(0, obstacles.Length)];
        spriteRender = GetComponent<SpriteRenderer>();
        collide = GetComponent<PolygonCollider2D>();
    }
	
	// Update is called once per frame
	void Update () {

        transform.position += new Vector3(0, speed, 0);

        if (transform.position.y > spawnBounds().y)
        {
            spawnCalc();
        }
    }

    Vector3 spawnBounds()
    {
        Vector3 boundingVars = new Vector3(0, 1200, 0);
        Vector3 boundingArea = Camera.main.ScreenToWorldPoint(boundingVars);

        return boundingArea;
    }

    void spawnCalc()
    {
        int size = obstacles.Length;

        float scaleRandomizer = Random.Range(2, 3);
        transform.localScale = new Vector3(scaleRandomizer, scaleRandomizer);

        currObstacle = obstacles[Random.Range(0, size)];

        spriteRender.sprite = currObstacle.GetComponent<SpriteRenderer>().sprite;
        collide.points = currObstacle.GetComponent<PolygonCollider2D>().points;

        Vector3 spawnPos = new Vector3(Random.Range(0, 640), -500, 9);
        transform.position = Camera.main.ScreenToWorldPoint(spawnPos);
    }
}
