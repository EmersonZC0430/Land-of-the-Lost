using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Grunt : EnemyController
{
    [Header("Skill")]
    public float kickFoirce = 10;/* 击飞力
     */


    public void Kickoff()
    {
        if (attackTarget != null)

        {
            transform.LookAt(attackTarget.transform);
            Vector3 direction = attackTarget.transform.position - transform.position;
            direction.Normalize();/* 量化0  1 -1 */
            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;
            attackTarget.GetComponent<NavMeshAgent>().velocity = direction * kickFoirce;

            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");

        }
    }
}
