using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("세팅")]
    public Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    [Header("적 세팅")]
    public float detectionRange = 15f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    private float lastAttackTime;
    public GameObject fistColliderObject;
    public float activeDuration = 0.2f;
    public bool haveMelee;
    public bool havePistol;
    public Weapon equippedWeapon;

    [Header("애니메이션")]
    public string speedParam = "forwardSpeed";
    public float animationMaxSpeed = 3f;   
    public float agentMaxSpeed = 3f;       

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        if (haveMelee) { animator.SetBool("haveMelee", true); }
        if (havePistol) { animator.SetBool("havePistol", true); }

        if (havePistol)
            attackRange = 15f; // 권총은 멀리서 공격 가능
        else if (haveMelee)
            attackRange = 1.5f; // 근접 무기
        else
            attackRange = 1.5f; // 맨손 기본

    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            if (distance > attackRange)
            {
                ChasePlayer();
            }
            else
            {
                AttackPlayer();
            }
        }
        else
        {
            Idle();
        }        
        UpdateAnimatorSpeed();
    }

    void ChasePlayer()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    void AttackPlayer()
    {
        agent.isStopped = true;
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        // 공격 애니메이션
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;

            //if (animator != null)
            if (!haveMelee && !havePistol)
            {
                Debug.Log("퍽!");
                animator.SetTrigger("Punching");
            }
            else if(havePistol)
            {
                Debug.Log("탕!");
                animator.SetTrigger("Shooting");
                equippedWeapon.EnemyAttack(player);
            }
            else if(haveMelee)
            {
                animator.SetTrigger("Swing");
            }                        
        }
    }

    void Idle()
    {
        agent.isStopped = true;
    }

    void UpdateAnimatorSpeed()
    {
        if (animator == null || agent == null) return;

        float currentSpeed = agent.velocity.magnitude;
        
        float mappedSpeed = Mathf.Clamp01(currentSpeed / agentMaxSpeed) * animationMaxSpeed;

        animator.SetFloat(speedParam, mappedSpeed);
    }    
}
