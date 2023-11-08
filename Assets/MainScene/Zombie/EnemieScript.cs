using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemieScript : MonoBehaviour
{
    GameObject commander;
    NavMeshAgent agent;
    GameObject[] civilians;
    // Start is called before the first frame update
    void Start()
    {
        //Only if he hears something
        commander = GameObject.FindWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        //agent.destination= commander.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // We need to check if the zombi sees a commander or a civilian
        float distance= 10000000000000000000;
        GameObject target_civilian = null;
        //The zombi will go to the closest civilian he sees
        civilians=GameObject.FindGameObjectsWithTag("Civilian");
        if (civilians!= null){
            foreach(GameObject civilian in civilians){
                Vector3 direction = gameObject.transform.position- civilian.transform.position;
                float angle= Vector3.Angle(direction, gameObject.transform.forward);
                if (angle < 45){
                    //if he sees him
                    Vector3 enemie_distance = civilian.transform.position - gameObject.transform.position;
                    float currentDistance = enemie_distance.sqrMagnitude;
                    if (currentDistance<distance && currentDistance<3000){
                        target_civilian= civilian;
                        distance=currentDistance;
                    }           
                }
            }
        }
        if (target_civilian != null){
            agent.destination= target_civilian.transform.position;
        }
        
    }

    public void Investigate(Vector3 position){
        Debug.Log("Zombi heard something");
        agent.destination = position;
    }
    public void Chase(NavMeshAgent chased){
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.destination= commander.transform.position;
    }
}
