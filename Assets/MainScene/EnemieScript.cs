using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemieScript : MonoBehaviour
{
    GameObject commander;
    // Start is called before the first frame update
    void Start()
    {
        commander = GameObject.FindWithTag("Player");
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.destination= commander.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        Quaternion targetRotation = Quaternion.LookRotation(commander.transform.position - transform.position);
        //Get the commander position and look that way
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1 * Time.deltaTime);
        //Changes where it is looking at every frame between its current look and where the commander is
        transform.position += transform.forward * 1f * Time.deltaTime;
        //move towards where it is looking, ideally it is where the player is
        */
    }
}
