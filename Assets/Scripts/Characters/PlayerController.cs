using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class PlayerController : MonoBehaviour
{


    private NavMeshAgent agent;
    private Animator anim;

    private CharacterStats characterStats;

    private GameObject attackTarget;
    /* cd冷却 */
    private float lastAttackTime;
    private float stopDistance;
    private bool isDead;
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();

        stopDistance = agent.stoppingDistance;
    }
    public void MoveToTarget(Vector3 target)
    {



        /* 停止所有协程（打断攻击） */
        StopAllCoroutines();

        if (isDead) return;


        agent.stoppingDistance = stopDistance;
        agent.isStopped = false;


        /* 注册到mousemanager */
        agent.destination = target;
    }
    // Start is called before the first frame update
    void Start()
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.OnEnemyClicked += EventAttack;
        GameManager.Instance.RigisterPlayer(characterStats);
    }


    // Update is called once per frame
    void Update()
    {
        isDead = characterStats.CurrentHealth == 0;
        if (isDead)
        {
            GameManager.Instance.NotifyObservers();

        }
        SwitchAnimation();
        lastAttackTime -= Time.deltaTime;
    }


    private void SwitchAnimation()
    {
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
        anim.SetBool("Death", isDead);
    }
    /*攻击 */
    private void EventAttack(GameObject target)
    {
        if (isDead) return;
        if (target != null)
        {
            attackTarget = target;

            characterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.criticalChance;
            /* 循环判断我当前目标与我之间的距离。距离>可攻击距离就得继续移动
 */
            StartCoroutine(MoveToAttackTarget());
        }


    }


    /* 协程 */
    IEnumerator MoveToAttackTarget()
    {/* 没有停止 */
        agent.isStopped = false;
        agent.stoppingDistance = characterStats.attackData.attackRange;
        transform.LookAt(attackTarget.transform);
        if (attackTarget == null)
            yield break;
        while (Vector3.Distance(attackTarget.transform.position, transform.position) > characterStats.attackData.attackRange)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;
            /* 代表在下一帧继续执行上面的指令 */
        }
        agent.isStopped = true;
        /* Attack */
        if (lastAttackTime < 0)
        {
            anim.SetBool("Critical", characterStats.isCritical);
            anim.SetTrigger("Attack");
            /* 重置CD */
            lastAttackTime = characterStats.attackData.coolDown;

        }
    }

    /* Animation Event */
    void Hit()
    {
        if (attackTarget.CompareTag("Attackable"))
        {
            if (attackTarget.GetComponent<GolemRock>() && attackTarget.GetComponent<GolemRock>().rockStates == GolemRock.RockStates.HitNothing)
            {
                attackTarget.GetComponent<GolemRock>().rockStates = GolemRock.RockStates.HitEney;
                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);
            }
        }
        else
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();

            targetStats.TakeDamge(characterStats, targetStats);
        }
    }

}
