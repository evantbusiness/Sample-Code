using UnityEngine;

public class cameraScript : MonoBehaviour {

    public float speed = 1.0f;

    void Start()
    {
        Screen.SetResolution(640, 960, false);
    }

    void Update () {
	    transform.position -= new Vector3(0, speed, 0);
	}
}
