using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    [SerializeField]
    private SphereCollider colliderVolume;

    private Vector3 impactForce;
    private float timeActive;
    private float delayTime;
    private float presetTimeActive;
    private int damage;
    private bool isActive;
    private bool delayedActivation;

    public Vector3 ImpactForce
    {
        get { return impactForce; }
    }

    public int Damage
    {
        get { return damage; }
    }

    private void Update()
    {
        if (delayTime > 0)
            delayTime -= Time.deltaTime;
        else if(delayTime <= 0 && delayedActivation)
        {
            timeActive = presetTimeActive;

            delayedActivation = false;

            isActive = true;
            colliderVolume.enabled = true;
        }

        if (timeActive >= 0)
            timeActive -= Time.deltaTime;
        else if (isActive)
            ResetCollider();
    }

    //Activate collider with time and stats
    public void ActivateCollider(float timeActive, float radius, int damage, Vector3 impactForce)
    {
        this.timeActive = timeActive;
        colliderVolume.radius = radius;
        this.damage = damage;
        this.impactForce = impactForce;

        isActive = true;
        this.gameObject.SetActive(true);
    }

    //Activate collider with delay
    public void ActivateCollider(float delayTime, float presetTimeActive, float radius, int damage, Vector3 impactForce)
    {
        this.delayTime = delayTime;
        this.presetTimeActive = presetTimeActive;
        colliderVolume.radius = radius;
        this.damage = damage;
        this.impactForce = impactForce;

        delayedActivation = true;

        colliderVolume.enabled = false;
        this.gameObject.SetActive(true);
    }

    //Force collider to stop if necessary (ex: if player is hit while attacking, the hitbox should not stay active)
    public void CancelCollider()
    {
        delayTime = 0;
        delayedActivation = false;
        timeActive = 0;
        presetTimeActive = 0;

        this.gameObject.SetActive(false);
        isActive = false;
    }

    //Turn off the collider when finished
    private void ResetCollider()
    {
        this.gameObject.SetActive(false);
        isActive = false;
    }
}
