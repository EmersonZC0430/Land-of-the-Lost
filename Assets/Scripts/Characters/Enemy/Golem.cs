using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Golem : EnemyController
{
    [Header("Skill")]
    public float kickFoirce = 25;/* 击飞力
     */
    public GameObject rockPrefab;
    public Transform handPos;
    public void KickOff()
    {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();

            Vector3 direction = attackTarget.transform.position - transform.position;
            direction.Normalize();/* 量化0  1 -1 */
            targetStats.GetComponent<NavMeshAgent>().isStopped = true;
            targetStats.GetComponent<NavMeshAgent>().velocity = direction * kickFoirce;

            targetStats.GetComponent<Animator>().SetTrigger("Dizzy");

            targetStats.TakeDamge(characterStats, targetStats);
        }
    }


    public void ThrowRock()
    {
        if (attackTarget != null)
        {
            var rock = Instantiate(rockPrefab, handPos.position, Quaternion.identity);

            rock.GetComponent<GolemRock>().target = attackTarget;




        }
    }

}
