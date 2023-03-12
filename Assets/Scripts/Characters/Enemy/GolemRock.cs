using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class GolemRock : MonoBehaviour
{

    public enum RockStates { HitPlayer, HitEney, HitNothing };

    public RockStates rockStates;

    private Rigidbody rb;
    [Header("Basic Settings")]
    public float force;

    public GameObject target;

    private Vector3 direction;
    public int damage;


    public void FlyToTarget()
    {

        if (target == null)
        {
            target = FindObjectOfType<PlayerController>().gameObject;

        }
        direction = (target.transform.position - transform.position + Vector3.up).normalized;

        rb.AddForce(direction * force, ForceMode.Impulse);


    }

    void FixedUpdate()
    {
        if (rb.velocity.sqrMagnitude < 1f)
        {
            rockStates = RockStates.HitNothing;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        switch (rockStates)
        {
            case RockStates.HitPlayer:
                if (other.gameObject.CompareTag("Player"))
                {
                    other.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    other.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force;
                    other.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");
                    other.gameObject.GetComponent<CharacterStats>().TakeDamge(damage, other.gameObject.GetComponent<CharacterStats>());

                    rockStates = RockStates.HitNothing;

                }
                break;
            case RockStates.HitEney:
                if (other.gameObject.GetComponent<Golem>())
                {
                    var otherStats = other.gameObject.GetComponent<CharacterStats>();

                    otherStats.TakeDamge(damage, otherStats);

                    Destroy(gameObject);

                }
                break;
            case RockStates.HitNothing:

                break;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.one;
        rockStates = RockStates.HitPlayer;
        FlyToTarget();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
