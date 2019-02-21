using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class characterControls : MonoBehaviour {

    public float tiltAngle;
    public float smooth;
    public float rotationLimit;
    public float xClamp;
    public float timePassed;
    public GameObject scoreUI;
    public GameObject audioController;
    public AudioClip collect;
    public AudioClip crash;
    private int life;

    // Use this for initialization
    void Start () {
        tiltAngle = 30.0f;
        smooth = 10.0f;
        rotationLimit = 40.0f;
        life = 1;
        timePassed = Time.time;
        mixLevels mixer = audioController.GetComponent<mixLevels>();
        mixer.startMusic();
    }
	
	// Update is called once per frame
	void Update () {
        posCalc();
    }

    void posCalc()
    {
        if(life > 0)
        {
            xClamp = Mathf.Clamp(Input.mousePosition.x, 0, 640);

            Vector3 mousePos = new Vector3(xClamp, 650, 10);

            float tiltAroundZ = (Input.GetAxis("Mouse X") * tiltAngle) % rotationLimit;
            float tiltAroundX = (Input.GetAxis("Mouse Y") * tiltAngle) % rotationLimit;
            Quaternion target = Quaternion.Euler(0, 0, tiltAroundZ);

            transform.position = Camera.main.ScreenToWorldPoint(mousePos);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);
        } else
        {
            transform.position -= new Vector3(0, 0.7f, 0);
            if (transform.position.y > deathBounds().y)
            {
                SceneManager.LoadScene(5);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collide)
    {
        if(collide.gameObject.tag == "treasure")
        {
            Text scoreNum = scoreUI.GetComponent<Text>();
            collectableSpawner treasure = collide.gameObject.GetComponent<collectableSpawner>();
            int numConvert = int.Parse(scoreNum.text);
            int displayValue = numConvert + treasure.collectableValue;
            scoreNum.text = displayValue.ToString();
            GetComponent<AudioSource>().PlayOneShot(collect, 0.25f);
            treasure.spawnCalc();
        }

        if (collide.gameObject.tag == "obstacle")
        {
            GetComponent<AudioSource>().PlayOneShot(crash, 0.7f);
            mixLevels mixer = audioController.GetComponent<mixLevels>();
            mixer.mute();
            life -= 1;
        }
    }

    Vector3 deathBounds()
    {
        Vector3 deathBounds = new Vector3(0, 2000, 0);
        Vector3 deathArea = Camera.main.ScreenToWorldPoint(deathBounds);

        return deathArea;
    }
}
