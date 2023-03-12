using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum EnemyStates { GUARD, PATROL, CHASE, DEAD }
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStats))]
public class EnemyController : MonoBehaviour, IEndGameObserver
{
    private EnemyStates enemyStates;
    private NavMeshAgent agent;


    private Animator anim;

    protected CharacterStats characterStats;
    private Collider coll;
    /* 追击 */
    [Header("Basic Settings")]
    public float sightRadius;
    public bool isGuard;

    private float speed;
    protected GameObject attackTarget;

    public float lookAtTime;
    private float remainLookAtTime;
    private float lastAttackTime;
    private Quaternion guardRotation;
    /* bool动画 */
    bool isWalk, isChase, isFollow, isDead, playerDead;


    [Header("Patrol State")]
    public float patrolRange;
    private Vector3 wayPoint;

    private Vector3 guardPos;
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        coll = GetComponent<Collider>();

        speed = agent.speed;
        guardPos = transform.position;//原始位置
        guardRotation = transform.rotation;//原始角度
        remainLookAtTime = lookAtTime;
    }
    /* 状态切换实现方法1.fsm状态机2.switch */
    void SwitchStates()
    {
        if (isDead)
        {
            enemyStates = EnemyStates.DEAD;
        }
        //如果发i西安player 就换到chase；
        else if (FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;

        }



        switch (enemyStates)
        {

            case EnemyStates.GUARD:
                GuardStates();
                break;
            case EnemyStates.PATROL:

                PatrolStates();
                break;

            case EnemyStates.CHASE:

                ChaseStates();

                break;
            case EnemyStates.DEAD:
                DeadStates();
                break;
        }
    }

    void GuardStates()
    {
        isChase = false;
        if (transform.position != guardPos)
        {
            isWalk = true;
            agent.isStopped = false;
            agent.destination = guardPos;
            /* 两个三位点的插值 */
            if (Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
            {
                isWalk = false;
                /* lerp 缓慢 */
                transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);
            }
        }
    }
    void DeadStates()
    {
        coll.enabled = false;
        /*  agent.enabled = false; */
        agent.radius = 0;
        Destroy(gameObject, 2f);
    }
    void PatrolStates()
    {
        isChase = false;
        agent.speed = speed * 0.5f;
        /* 判断是否到了巡逻点 */
        if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
        {
            isWalk = false;
            if (remainLookAtTime > 0)
            {
                remainLookAtTime -= Time.deltaTime;
            }
            else
            {
                GetNewWayPoint();
            }
        }
        else
        {
            isWalk = true;
            agent.destination = wayPoint;

        }
    }
    void ChaseStates()
    {


        /* 在攻击范围内侧攻击 */
        isWalk = false;
        isChase = true;
        agent.speed = speed;
        /* 配合动画 */
        if (!FoundPlayer())
        {

            /* 拉脱回到上一个状态 */
            isFollow = false;
            if (remainLookAtTime > 0)
            {
                agent.destination = transform.position;
                remainLookAtTime -= Time.deltaTime;
            }

            else if (isGuard)
            {
                enemyStates = EnemyStates.GUARD;
            }
            else
            {
                enemyStates = EnemyStates.PATROL;
            }
        }
        else
        {
            isFollow = true;
            agent.isStopped = false;
            agent.destination = attackTarget.transform.position;
        }
        /* 在范围攻击 */
        if (TargetInAttackRange() || TargetInSkillRange())
        {
            isFollow = false;
            agent.isStopped = true;
            if (lastAttackTime < 0)
            {
                lastAttackTime = characterStats.attackData.coolDown;

                //暴击
                characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;
                //执行攻击
                Attack();
            }
        }
    }


    void Attack()
    {
        transform.LookAt(attackTarget.transform);
        if (TargetInAttackRange())
        {
            //近身攻击动画
            anim.SetTrigger("Attack");
        }
        if (TargetInSkillRange())
        {
            //技能攻击动画
            anim.SetTrigger("Skill");
        }
    }

    bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);
        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }
        attackTarget = null;
        return false;
    }
    bool TargetInAttackRange()
    {
        if (attackTarget != null)
        {
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.attackRange;
        }
        else return false;
    }

    bool TargetInSkillRange()
    {
        if (attackTarget != null)
        {
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.skillRange;
        }
        else return false;
    }
    void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", characterStats.isCritical);
        anim.SetBool("Death", isDead);
    }
    void GetNewWayPoint()
    {
        remainLookAtTime = lookAtTime;

        float randomX = Random.Range(-patrolRange, patrolRange);

        float randomZ = Random.Range(-patrolRange, patrolRange);


        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y, guardPos.z + randomZ);
        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;

    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }
    // Start is called before the first frame update
    void Start()
    {
        if (isGuard)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            enemyStates = EnemyStates.PATROL;
            GetNewWayPoint();
        }
        GameManager.Instance.AddObservers(this);
    }
    //切换场景时启用
    /*     void OnEnable()
        {
            GameManager.Instance.AddObservers(this);
        } */
    void OnDisable()
    {
        if (!GameManager.IsInitialized) return;
        GameManager.Instance.RemoveObservers(this);

    }


    // Update is called once per frame
    void Update()
    {

        if (characterStats.CurrentHealth == 0)
        {
            isDead = true;
        }
        if (!playerDead)
        {
            SwitchStates();
            SwitchAnimation();

        }


        lastAttackTime -= Time.deltaTime;
    }

    /* Animation Event */
    void Hit()
    {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamge(characterStats, targetStats);
        }

    }

    public void EndNotify()
    {
        anim.SetBool("Win", true);
        playerDead = true;
        //获胜动画、停止活动
        isChase = false;
        isWalk = false;
        attackTarget = null;



    }
}
