using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// Currently trying to make it so that the civilian can only scape once he is close to a soldier for the specified ammount.
/// </summary>
public class CivillianScript : MonoBehaviour
{
    public NavMeshAgent agent;
    private Vector3 currentVelocity;
    float time_left;
    // Start is called before the first frame update
    void Start()
    {
        time_left = Random.Range(30f, 90f);
        agent = GetComponent<NavMeshAgent>();
        //Debug.Log(time_left);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 survivingV = DodgeZombies();
        //Debug.Log(gameObject.name+survivingV);
        currentVelocity = Vector3.Lerp(currentVelocity, survivingV, Time.deltaTime);
        agent.Move(currentVelocity * Time.deltaTime);
        //time_left=time_left-Time.deltaTime;
        GameObject[] soldiers;
        soldiers=GameObject.FindGameObjectsWithTag("Allie");
        bool close = false;
        foreach(GameObject soldier in soldiers){
            Vector3 distance = soldier.transform.position-gameObject.transform.position;
            if (distance.sqrMagnitude<20){
                close = true;
            }
        }
        if (close==true){
            time_left=time_left-Time.deltaTime;
        } else {
            time_left=Random.Range(30f, 90f);
            //Debug.Log("Too far away, time reseted");
        }
        if (time_left<=0){
            Debug.Log("Civillian saved");
            Destroy(gameObject);
        }
        
        
    }
    Vector3 DodgeZombies(){
        LayerMask zombieMask = LayerMask.GetMask("Zombie", "Walls");
        Collider[] zombies = Physics.OverlapSphere(transform.position, 30, zombieMask);
        Vector3 dodgeVector= Vector3.zero;
        foreach(var zombi in zombies){
            Vector3 avoidVector= new Vector3(transform.position.x-zombi.transform.position.x, 0, transform.position.z-zombi.transform.position.z);
            //Debug.Log(transform.position+""+avoidVector+""+zombi.transform.position);
            dodgeVector=dodgeVector+(avoidVector.normalized/avoidVector.magnitude);
        }
        return dodgeVector.normalized * 5;
    }
}
