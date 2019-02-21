using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour 
{
    [SerializeField]
    protected Rigidbody rigidBody;

    [SerializeField]
    protected Animator anim;

    [SerializeField]
    protected GameObject healthDrop;

    [SerializeField]
    protected ParticleSystem hitSpark;

    [SerializeField]
    protected ParticleSystem attackFlash;

    [SerializeField]
    protected ParticleSystem explosion;

    [SerializeField]
    protected List<GameObject> damageColliders;

    protected GameplayManager gameplayManager;

    protected Transform playerTransform;
    protected Vector3 enemyStartPos;
    protected float distanceToPlayer;
    protected float distanceToStart;

    protected EntityStats stats;

    protected int attractEnemyDist;
    protected int maxDistFromSpawn;
    protected float fightingDist;

    protected float attackDelay;

    protected bool canMove;
    protected bool canChase;

    protected enum EnemyState { Idle, Chasing, ReturnToSpawn, Fighting };
    protected EnemyState currentState;

    //Initialization
    public virtual void Start()
    {
        gameplayManager = GameObject.Find("GameplayManager").GetComponent<GameplayManager>();

        playerTransform = GameObject.Find("PlayerFinal").transform;
        enemyStartPos = this.transform.position;

        canMove = true;

        currentState = EnemyState.Idle;
    }

    //Update
    public virtual void FixedUpdate()
    {
        CheckEnemyState();

        //Enemy State Machine
        if (stats.applicableForce == Vector3.zero)
            switch (currentState)
            {
                case EnemyState.Chasing:
                    Chasing();
                    break;
                case EnemyState.ReturnToSpawn:
                    ReturnToSpawn();
                    break;
                case EnemyState.Idle:
                    Idle();
                    break;
                case EnemyState.Fighting:
                    Fighting();
                    break;
            }

        if (stats.applicableForce != Vector3.zero)
            ApplyForce();

        if (stats.invTime > 0)
            stats.invTime -= Time.deltaTime;
    }

    private void CheckEnemyState()
    {
        //Check distance from player and distance from start position
        distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        distanceToStart = Vector3.Distance(transform.position, enemyStartPos);

        //If enemy is close enough to player it starts to chase
        if (distanceToPlayer <= attractEnemyDist && canChase)
            currentState = EnemyState.Chasing;

        //If enemy is to far from spawn it should return to spawn
        if (distanceToStart >= maxDistFromSpawn)
        {
            currentState = EnemyState.ReturnToSpawn;
            canChase = false;
        }

        //If the enemy is close enough to the player stop chasing and start fighting
        if (distanceToPlayer <= fightingDist)
        {
            currentState = EnemyState.Fighting;
            canChase = true;
        }

        //If enemy returns to start become idle
        if (distanceToStart <= 1 && currentState != EnemyState.Chasing)
        {
            currentState = EnemyState.Idle;
            canChase = true;
        }
    }

    public virtual void Idle()
    {
        //Extend with individual animation/behavior
    }

    public virtual void Chasing()
    {
        //Look at and move toward player
        if(canMove)
        {
            transform.LookAt(new Vector3(playerTransform.position.x, this.transform.position.y, playerTransform.position.z));
            rigidBody.position += transform.forward * stats.movementSpeed * Time.deltaTime;
        }

        //Extend with individual animation/behavior
    }

    public virtual void ReturnToSpawn()
    {
        //Look at and return to spawn
        if(canMove)
        {
            transform.LookAt(new Vector3(enemyStartPos.x, this.transform.position.y, enemyStartPos.z));
            rigidBody.position += transform.forward * stats.movementSpeed * Time.deltaTime;
        }

        //Extend with individual animation/behavior
    }

    public virtual void Fighting()
    {
        if(stats.health > 0)
            transform.LookAt(new Vector3(playerTransform.position.x, this.transform.position.y, playerTransform.position.z));

        //Extend with individual animation/behavior
    }

    private void ApplyForce()
    {
        //How fast the impact resolves
        float impactSpeed = 5.0f;

        //Aproach zero to lose force over time
        stats.applicableForce = Vector3.Lerp(stats.applicableForce, Vector3.zero, Time.deltaTime * impactSpeed);
        rigidBody.MovePosition(transform.position + (transform.up * stats.applicableForce.y) + (transform.forward * stats.applicableForce.z));

        //Reset impact completely if close enough to zero so there is no remaining force
        if (Vector3.Distance(stats.applicableForce, Vector3.zero) < 0.05f)
            stats.applicableForce = Vector3.zero;
    }

    void OnTriggerEnter(Collider trigger)
    {
        //Entering a hitbox (could also be viewed as a punch being moved toward you)
        if (trigger.gameObject.tag == "Hitbox" && stats.invTime <= 0)
        {
            //Get the DamageCollider
            DamageCollider damageCollider = trigger.gameObject.GetComponent<DamageCollider>();

            //Impact effects
            SoundManager.Instance.PlaySound("Impact" + Random.Range(1,4).ToString());
            Instantiate(hitSpark, trigger.gameObject.transform.position, Quaternion.identity);
            gameplayManager.WorldImpactEffect(damageCollider.Damage * 0.01f, damageCollider.Damage * 0.005f, damageCollider.Damage * 0.004f);

            //Apply values
            stats.health -= damageCollider.Damage;
            stats.applicableForce = damageCollider.ImpactForce;

            stats.invTime = 0.2f;
        }
    }
}
