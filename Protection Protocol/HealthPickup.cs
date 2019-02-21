using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour 
{
    [SerializeField]
    private ParticleSystem collectionSplash;

	private int restoreAmount;

    private Vector3 dropPos;
    private Vector3 hoverPos;

    private bool isHoveringUp;

    public AudioClip healthPickup;

    public int RestoreAmount
    {
        get { return restoreAmount; }
    }

    private void Awake()
    {
        int restoreRange = Random.Range(0, 10);
        restoreAmount = 10 + restoreRange;

        dropPos = this.transform.position;
        hoverPos = new Vector3(dropPos.x, dropPos.y + 0.2f, dropPos.z);

        isHoveringUp = true;
    }

    private void Update () 
    {
        float hoverSpeed = Time.deltaTime/5;

        if (this.transform.position.y < dropPos.y)
            isHoveringUp = true;
        else if (this.transform.position.y > hoverPos.y)
            isHoveringUp = false;

        if(isHoveringUp)
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + hoverSpeed, this.transform.position.z);
        else
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - hoverSpeed, this.transform.position.z);

    }

    public void CollectPickup()
    {
        Instantiate(collectionSplash, this.transform.position, Quaternion.identity);
        SoundManager.Instance.PlaySound("HealthUpSound");
        Destroy(this.gameObject);
    }
}
