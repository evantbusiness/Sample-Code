using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rigidBody;

    [SerializeField]
    private Animator anim;

    [SerializeField]
    private ParticleSystem hitEffect;

    [SerializeField]
    private ParticleSystem guardEffect;

    [SerializeField]
    private GameObject dashSmoke;

    [SerializeField] private GameObject rightHandCollider;
    [SerializeField] private GameObject leftHandCollider;
    [SerializeField] private GameObject rightFootCollider;
    [SerializeField] private GameObject leftFootCollider;

    private GameplayManager gameplayManager;

    private EntityStats stats;

    private enum MovementType { Neutral, Guard, Dash };
    private MovementType currentStance;

    private Vector3 currentVelocity;
    private Vector3 lastPos;

    private bool canMove;
    private bool IsReturningToNeutral;
    
    private float turningSpeed;
    private bool turning;
    private Quaternion quickTurnTarget;

    private float dodgeSpeed;
    private float dodgePos;
    private float dodgeTarget;

    private Vector3 dashRemainder;
    private float standardDashSpeed;
    private float adjustingDashSpeed;

    private float h;
    private float v;

    private int punchValue;
    private int kickValue;

    public bool IsDashing
    {
        get { return anim.GetCurrentAnimatorStateInfo(0).IsTag("DashAnim"); }
    }

    public int Health
    {
        get { return stats.health; }
    }

    private bool IsAnimationFinished(string animName)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(animName) && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            return true;
        else
            return false;
    }

    //Initialization
    void Start () 
    {
        gameplayManager = GameObject.Find("GameplayManager").GetComponent<GameplayManager>();

        stats = new EntityStats();
        stats.health = 100;
        stats.baseDamage = 5;
        stats.movementSpeed = 10.0f;

        currentStance = MovementType.Neutral;

        currentVelocity = Vector3.zero;
        lastPos = rigidBody.position;

        canMove = true;
        IsReturningToNeutral = false;

        turningSpeed = 80;
        quickTurnTarget = transform.rotation;

        dodgeSpeed = 8.0f;
        dodgePos = 0.0f;
        dodgeTarget = 0.0f;

        standardDashSpeed = 25.0f;
        adjustingDashSpeed = 40.0f;

        punchValue = 0;
        kickValue = 0;
    }
    
    //Update
    void FixedUpdate () 
    {
        currentVelocity = (rigidBody.position - lastPos) * 1 / Time.fixedDeltaTime;
        lastPos = rigidBody.position;

        if(IsReturningToNeutral)
            if (!anim.GetCurrentAnimatorStateInfo(0).IsTag("PunchAnim") || !anim.GetCurrentAnimatorStateInfo(0).IsTag("KickAnim"))
            {
                punchValue = 0;
                kickValue = 0;
                anim.SetInteger("PunchValue", punchValue);
                anim.SetInteger("KickValue", kickValue);

                IsReturningToNeutral = false;
            }

        //Attack
        AttackHandler();

        //Movement
        CheckStance();

        if(canMove)
            switch(currentStance)
            {
                case MovementType.Neutral:
                    NeutralMovement();
                    break;
                case MovementType.Guard:
                    GuardMovement();
                    break;
                case MovementType.Dash:
                    DashMovement();
                    break;
            }

        //Resolve leftover velocity from dash by slowing to a stop
        if(Vector3.Distance(dashRemainder, Vector3.zero) > 0.05 && currentStance != MovementType.Dash)
        {
            dashRemainder = Vector3.Lerp(dashRemainder, Vector3.zero, Time.deltaTime * 4);
            rigidBody.velocity = new Vector3(dashRemainder.x, Physics.gravity.y, dashRemainder.z);
        }

        //Quick Turn
        if (Input.GetButtonDown("QuickTurn"))
        {
            quickTurnTarget = Quaternion.Euler(0, transform.rotation.eulerAngles.y + 180, 0);
            turning = true;
        }

        if (currentStance == MovementType.Neutral && Vector3.Distance(transform.rotation.eulerAngles, quickTurnTarget.eulerAngles) > 5.0f && turning)
            transform.rotation = Quaternion.Lerp(rigidBody.rotation, quickTurnTarget, Time.deltaTime * 5);
        else
            turning = false;

        //Apply force
        if(stats.applicableForce != Vector3.zero)
            ApplyForce();

        if (stats.invTime > 0)
            stats.invTime -= Time.deltaTime;

        if (stats.health <= 0)
            anim.SetBool("IsDead", true);

        if(IsAnimationFinished("Death"))
            gameplayManager.GetComponent<GameplayManager>().GameOver();

    }

    private void AttackHandler()
    {
        NeutralRevertCheck();

        if (Input.GetButtonDown("Punch") && !IsReturningToNeutral)
        {
            if(currentStance == MovementType.Neutral)
            {
                if (punchValue < 3 && kickValue == 0)
                {
                    if (anim.GetCurrentAnimatorStateInfo(0).IsName("Punch_" + punchValue))
                        punchValue += 1;
                    else if (anim.GetCurrentAnimatorStateInfo(0).IsTag("ActionableAnim"))
                        punchValue = 1;

                    anim.SetInteger("PunchValue", punchValue);
                }

                if (kickValue > 0 && !anim.GetBool("HeavyKick"))
                {
                    anim.SetBool("HeavyPunch", true);
                    rightHandCollider.GetComponent<DamageCollider>().ActivateCollider(1.5f, 0.4f, stats.baseDamage * 5, new Vector3(0, 0, -0.5f));
                    return;
                }

                switch(punchValue)
                {
                    case 1:
                        rightHandCollider.GetComponent<DamageCollider>().ActivateCollider(0.5f, 0.3f, stats.baseDamage * punchValue, new Vector3(0, 0, -0.05f));
                        break;
                    case 2:
                        leftHandCollider.GetComponent<DamageCollider>().ActivateCollider(0.5f, 0.3f, stats.baseDamage * punchValue, new Vector3(0, 0, -0.05f));
                        break;
                    case 3:
                        rightHandCollider.GetComponent<DamageCollider>().ActivateCollider(0.5f, 0.3f, stats.baseDamage * punchValue, new Vector3(0, 0, -0.06f));
                        break;
                    default:
                        break;
                }
            }
            else if(currentStance == MovementType.Dash)
            {
                anim.SetBool("DashPunch", true);
                rightHandCollider.GetComponent<DamageCollider>().ActivateCollider(1.5f, 1.0f, stats.baseDamage * 8, new Vector3(0, 0, -1.2f));
            }
        }
        else if(Input.GetButtonDown("Kick") && !IsReturningToNeutral)
        {
            if (currentStance == MovementType.Neutral)
            {
                if (kickValue < 3 && punchValue == 0)
                {
                    if (anim.GetCurrentAnimatorStateInfo(0).IsName("Kick_" + kickValue))
                        kickValue += 1;
                    else if (anim.GetCurrentAnimatorStateInfo(0).IsTag("ActionableAnim"))
                        kickValue = 1;

                    anim.SetInteger("KickValue", kickValue);
                }

                if (punchValue > 0 && !anim.GetBool("HeavyPunch"))
                {
                    anim.SetBool("HeavyKick", true);
                    rightFootCollider.GetComponent<DamageCollider>().ActivateCollider(1.5f, 0.4f, stats.baseDamage * 6, new Vector3(0, 0.35f, -0.5f));
                    return;
                }

                switch (kickValue)
                {
                    case 1:
                        rightFootCollider.GetComponent<DamageCollider>().ActivateCollider(0.5f, 0.3f, stats.baseDamage * (kickValue + 1), new Vector3(0, 0, -0.05f));
                        break;
                    case 2:
                        leftFootCollider.GetComponent<DamageCollider>().ActivateCollider(0.5f, 0.3f, stats.baseDamage * (kickValue + 1), new Vector3(0, 0, -0.05f));
                        break;
                    case 3:
                        rightFootCollider.GetComponent<DamageCollider>().ActivateCollider(0.5f, 0.3f, stats.baseDamage * (kickValue + 1), new Vector3(0, 0, -0.07f));
                        break;
                    default:
                        break;
                }
            }
            else if (currentStance == MovementType.Dash)
            {
                anim.SetBool("DashKick", true);
                rightFootCollider.GetComponent<DamageCollider>().ActivateCollider(0.5f, 1.0f, stats.baseDamage * 10, new Vector3(0, 1.2f, -0.9f));
            }
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("PunchAnim") || anim.GetCurrentAnimatorStateInfo(0).IsTag("KickAnim") || anim.GetCurrentAnimatorStateInfo(0).IsName("Death"))
            canMove = false;
        else
            canMove = true;
    }

    private void NeutralRevertCheck()
    {
        if (IsAnimationFinished("Punch_" + punchValue) && punchValue == anim.GetInteger("PunchValue"))
            IsReturningToNeutral = true;

        if (IsAnimationFinished("Kick_" + kickValue) && kickValue == anim.GetInteger("KickValue"))
            IsReturningToNeutral = true;

        if (IsAnimationFinished("Heavy_Punch"))
        {
            IsReturningToNeutral = true;
            anim.SetBool("HeavyPunch", false);
        }

        if (IsAnimationFinished("Heavy_Kick"))
        {
            IsReturningToNeutral = true;
            anim.SetBool("HeavyKick", false);
        }

        if (IsAnimationFinished("Dash_Punch"))
        {
            IsReturningToNeutral = true;
            anim.SetBool("DashPunch", false);
            anim.SetBool("IsDashing", false);
            anim.SetBool("IsCharging", false);
            dashSmoke.SetActive(false);
            adjustingDashSpeed = 40.0f;
            dashRemainder = Vector3.zero;
            currentStance = MovementType.Neutral;
        }

        if (IsAnimationFinished("Dash_Kick"))
        {
            IsReturningToNeutral = true;
            anim.SetBool("DashKick", false);
            anim.SetBool("IsDashing", false);
            anim.SetBool("IsCharging", false);
            dashSmoke.SetActive(false);
            adjustingDashSpeed = 40.0f;
            dashRemainder = Vector3.zero;
            currentStance = MovementType.Neutral;
        }
    }

    private void CheckStance()
    {
        //Guard check
        if (Input.GetButton("Guard") && currentStance == MovementType.Neutral)
        {
            anim.SetBool("IsWalking", false);
            anim.SetBool("IsGuarding", true);
            h = 0;
            currentStance = MovementType.Guard;
        }
        else if (!Input.GetButton("Guard") && currentStance == MovementType.Guard && dodgeTarget == 0)
        {
            anim.SetBool("IsGuarding", false);
            anim.SetBool("IsDodging", false);
            currentStance = MovementType.Neutral;
        }

        //Dash Check
        if(Input.GetAxis("Dash") > 0 && currentStance == MovementType.Neutral)
        {
            anim.SetBool("IsWalking", false);
            anim.SetBool("IsCharging", true);
            currentStance = MovementType.Dash;
        }
        else if (Input.GetAxis("Dash") <= 0 && currentStance == MovementType.Dash)
        {
            anim.SetBool("IsDashing", false);
            anim.SetBool("IsCharging", false);
            dashSmoke.SetActive(false);
            adjustingDashSpeed = 40.0f;
            currentStance = MovementType.Neutral;
        }
    }

    private void NeutralMovement()
    {
        //Input
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        if (v != 0)
        {
            anim.SetFloat("WalkingValue", v);
            anim.SetBool("IsWalking", true);
        }
        else
            anim.SetBool("IsWalking", false);

        //Apply movement
        if(v != 0)
        {
            rigidBody.MoveRotation(Quaternion.Euler(0, rigidBody.rotation.eulerAngles.y + (h * turningSpeed * Time.deltaTime), 0));

            if (v > 0)
                rigidBody.MovePosition(transform.position + (transform.forward * stats.movementSpeed * Time.deltaTime));
            else if (v < 0)
                rigidBody.MovePosition(transform.position + (transform.forward * -(stats.movementSpeed * Time.deltaTime) / 3));
        }
        else
            rigidBody.MoveRotation(Quaternion.Euler(0, rigidBody.rotation.eulerAngles.y + (h * turningSpeed * Time.deltaTime)*2, 0));
    }

    private void GuardMovement()
    {
        //Input
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Guard"))
            h = Input.GetAxis("Horizontal");

        //If delay is up
        if(rigidBody.velocity == Vector3.zero)
        {
            if (h > 0)
            {
                dodgeTarget = 1.45f;
                anim.SetFloat("DodgeValue", dodgeTarget);
                anim.SetBool("IsDodging", true);
            }
            else if (h < 0)
            {
                dodgeTarget = -1.45f;
                anim.SetFloat("DodgeValue", dodgeTarget);
                anim.SetBool("IsDodging", true);
            }
        }

        if (dodgeTarget == 0)
        {
            anim.SetFloat("DodgeValue", dodgeTarget);
            anim.SetBool("IsDodging", false);
        }

        //Dodging until approximate target is met
        if (dodgeTarget < 0 && dodgePos > dodgeTarget + 0.5f || dodgeTarget > 0 && dodgePos < dodgeTarget - 0.5f)
        {
            dodgePos = Mathf.Lerp(dodgePos, dodgeTarget, Time.deltaTime * dodgeSpeed);
            rigidBody.velocity += transform.right * dodgePos;
        }
        else
        {
            //Reset target and dodge position when dodge has finished
            dodgeTarget = 0.0f;
            dodgePos = 0.0f;
        }
    }

    private void DashMovement()
    {
        //Input
        h = Input.GetAxis("Horizontal");

        float hMove = (h * stats.movementSpeed * Time.deltaTime)/2;

        //If charging animation has finished, the player can dash
        if (IsAnimationFinished("Charge") && !anim.GetBool("IsDashing"))
        {
            anim.SetBool("IsDashing", true);
            dashSmoke.SetActive(true);
            SoundManager.Instance.PlaySound("Explosion2");
        }

        //Apply movement while dashing & not interrupted
        if (anim.GetBool("IsDashing") && stats.applicableForce == Vector3.zero)
        {
            //Start fast and reach normal dashing speed over time so the release feels like a burst forwards
            adjustingDashSpeed = Mathf.Lerp(adjustingDashSpeed, standardDashSpeed, Time.deltaTime * 4);
            rigidBody.MovePosition(transform.position + (transform.forward * adjustingDashSpeed * Time.deltaTime) + (transform.right * hMove));
            dashRemainder = currentVelocity;
        }

        if (stats.applicableForce != Vector3.zero)
        {
            anim.SetBool("IsDashing", false);
            anim.SetBool("IsCharging", false);
            dashSmoke.SetActive(false);
            adjustingDashSpeed = 40.0f;
        }
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
        {
            anim.SetBool("IsStaggered", false);
            anim.SetBool("IsKnockedBack", false);
            stats.applicableForce = Vector3.zero;

            IsReturningToNeutral = true;
            anim.SetBool("HeavyPunch", false);
            anim.SetBool("HeavyKick", false);
            anim.SetBool("DashPunch", false);
            anim.SetBool("DashKick", false);
            anim.SetBool("IsDashing", false);
            anim.SetBool("IsCharging", false);
            dashSmoke.SetActive(false);
            adjustingDashSpeed = 40.0f;
            dashRemainder = Vector3.zero;

            currentStance = MovementType.Neutral;
        }
        else if (stats.applicableForce.magnitude > 0.05f && stats.applicableForce.y == 0)
        {
            anim.SetBool("IsStaggered", true);

            rightHandCollider.GetComponent<DamageCollider>().CancelCollider();
            leftHandCollider.GetComponent<DamageCollider>().CancelCollider();
            rightFootCollider.GetComponent<DamageCollider>().CancelCollider();
            leftFootCollider.GetComponent<DamageCollider>().CancelCollider();
        }
        else if (stats.applicableForce.y > 0)
        {
            anim.SetBool("IsKnockedBack", true);

            rightHandCollider.GetComponent<DamageCollider>().CancelCollider();
            leftHandCollider.GetComponent<DamageCollider>().CancelCollider();
            rightFootCollider.GetComponent<DamageCollider>().CancelCollider();
            leftFootCollider.GetComponent<DamageCollider>().CancelCollider();
        }
    }

    void OnTriggerEnter(Collider trigger)
    {
        //Entering a hurtbox (could also be viewed as a punch being moved toward you)
        if (trigger.gameObject.tag == "Hurtbox" && stats.invTime <= 0)
        {
            //Get the DamageCollider
            DamageCollider damageCollider = trigger.gameObject.GetComponent<DamageCollider>();

            //Apply values
            if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Guard"))
            {
                SoundManager.Instance.PlaySound("Impact" + Random.Range(1, 4).ToString());
                Instantiate(hitEffect, damageCollider.transform.position, Quaternion.identity);
                gameplayManager.WorldImpactEffect(damageCollider.Damage * 0.01f, damageCollider.Damage * 0.005f, damageCollider.Damage * 0.004f);

                stats.health -= damageCollider.Damage;
                stats.applicableForce = damageCollider.ImpactForce;
            }
            else
            {
                SoundManager.Instance.PlaySound("ImpactOnShield");
                Instantiate(guardEffect, this.transform.position, Quaternion.identity);
                gameplayManager.WorldImpactEffect(damageCollider.Damage * 0.01f, damageCollider.Damage * 0.002f, damageCollider.Damage * 0.001f);

                stats.health -= (damageCollider.Damage) / 3;
            }

            stats.invTime = 0.5f;
        }

        if (trigger.gameObject.tag == "Health")
        {
            stats.health += trigger.gameObject.GetComponent<HealthPickup>().RestoreAmount;
            stats.health = Mathf.Clamp(stats.health, 0, 100);

            trigger.gameObject.GetComponent<HealthPickup>().CollectPickup();
        }

        if (trigger.gameObject.tag == "LevelEnd")
        {
            gameplayManager.NextLevel();
        }
    }

    //Knockback from dashing into wall
    private void OnCollisionEnter(Collision collision)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Dash") && collision.gameObject.tag != "Terrain")
        {
            gameplayManager.GetComponent<GameplayManager>().WorldImpactEffect(0, 0.1f, 0.2f);
            stats.applicableForce = new Vector3(0, 0, -0.2f);
            SoundManager.Instance.PlaySound("Impact2");
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Dash") && collision.gameObject.tag != "Terrain")
        {
            gameplayManager.GetComponent<GameplayManager>().WorldImpactEffect(0, 0.1f, 0.2f);
            stats.applicableForce = new Vector3(0, 0, -0.2f);
            SoundManager.Instance.PlaySound("Impact2");
        }
    }
}
