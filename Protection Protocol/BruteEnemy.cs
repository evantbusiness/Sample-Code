using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BruteEnemy : Enemy
{
    //Initialize
    public override void Start()
    {
        //Brute values
        stats = new EntityStats();
        stats.health = 150;
        stats.baseDamage = 8;
        stats.movementSpeed = 6.0f;

        //AI Params
        attractEnemyDist = 10;
        maxDistFromSpawn = 20;
        fightingDist = 2.55f;

        base.Start();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        //Death
        if (stats.health <= 0 && stats.applicableForce == Vector3.zero)
            anim.SetBool("IsDead", true);

        if(anim.GetCurrentAnimatorStateInfo(0).IsName("Death") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            int dropRate = Random.Range(0, 5);

            if (dropRate == 2)
                Instantiate(healthDrop, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Quaternion.identity);

            Instantiate(explosion, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Quaternion.identity);
            SoundManager.Instance.PlaySound("Explosion1");

            Destroy(this.gameObject);
        }

        if (attackDelay > 0)
            attackDelay -= Time.deltaTime;

        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("MoveableAnim"))
            canMove = true;
        else
            canMove = false;

        if (stats.applicableForce.magnitude >= 0.1f)
            anim.SetBool("IsKnockedBack", true);
        else if(stats.applicableForce.magnitude < 0.15f && anim.GetBool("IsKnockedBack"))
            anim.SetBool("IsKnockedBack", false);

        NeutralRevert();
    }

    private void NeutralRevert()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("AttackAnim") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            anim.SetInteger("AttackValue", 0);
    }

    //Unique enemy behavior
    #region Behavior

    public override void Idle()
    {
        base.Idle();

        if(anim.GetCurrentAnimatorStateInfo(0).IsName("Run"))
            anim.SetBool("IsWalking", false);
    }

    public override void Chasing()
    {
        base.Chasing();

        if(anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            anim.SetBool("IsWalking", true);
    }

    public override void ReturnToSpawn()
    {
        base.ReturnToSpawn();

        anim.SetBool("IsWalking", true);
    }

    public override void Fighting()
    {
        base.Fighting();

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Run"))
            anim.SetBool("IsWalking", false);

        int attackCheck = Random.Range(0, 200);

        if(attackCheck == 100 && attackDelay <= 0 && !anim.GetCurrentAnimatorStateInfo(0).IsTag("AttackAnim") && !anim.GetBool("IsDead"))
        {
            Instantiate(attackFlash, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Quaternion.identity);

            int attackSelection = Random.Range(1, 4);

            anim.SetInteger("AttackValue", attackSelection);
            
            switch(attackSelection)
            {
                case 1:
                    damageColliders[0].GetComponent<DamageCollider>().ActivateCollider(0.2f, 0.3f, 0.17f, stats.baseDamage, new Vector3(0, 0, -0.08f));
                    break;
                case 2:
                    damageColliders[1].GetComponent<DamageCollider>().ActivateCollider(0.4f, 0.4f, 0.2f, stats.baseDamage * 2, new Vector3(0, 0.08f, -0.2f));
                    break;
                case 3:
                    damageColliders[2].GetComponent<DamageCollider>().ActivateCollider(0.2f, 0.3f, 0.12f, stats.baseDamage + 5, new Vector3(0, 0, -0.25f));
                    break;
                default:
                    Debug.Log("Selected attack outside of valid range");
                    break;
            }

            attackDelay = 2.0f;
        }
    }

    #endregion
}
