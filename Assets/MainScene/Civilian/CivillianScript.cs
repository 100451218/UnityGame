using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class CivillianScript : MonoBehaviour
{
    //Variable declaration
    public NavMeshAgent agent;
    private Vector3 currentVelocity;
    float time_left;
    void Start()
    {
        //Set the time the civilian needs to survive to be saved between 30 and 90 seconds
        time_left = Random.Range(30f, 90f);
        //Set the navMeshAgent
        agent = GetComponent<NavMeshAgent>();
        //Debug.Log(time_left);
    }

    // Update is called once per frame
    void Update()
    {
        //We calculate a vector force to be applied for the civilian to survive
        Vector3 survivingV = DodgeZombies();
        //Debug.Log(gameObject.name+survivingV);
        //We need to change the current force to a combinaton of where he wants to go and where he should go
        currentVelocity = Vector3.Lerp(currentVelocity, survivingV, Time.deltaTime);
        //We move the civilian to that new force
        agent.Move(currentVelocity * Time.deltaTime);
        //time_left=time_left-Time.deltaTime;

        //Now that he moved we verify if he is next enough to a turret
        GameObject[] soldiers;
        soldiers=GameObject.FindGameObjectsWithTag("Allie");
        bool close = false;
        foreach(GameObject soldier in soldiers){
            //if he is at least 30 units close to a turret then he is close enough
            Vector3 distance = soldier.transform.position-gameObject.transform.position;
            if (distance.sqrMagnitude<30){
                close = true;
            }
        }
        if (close==true){
            //If he is close enough he can be closer to be saved
            time_left=time_left-Time.deltaTime;
        } else {
            //If he leaves the area of the turret then we reset the time ( to a new one)
            time_left=Random.Range(30f, 90f);
            //Debug.Log("Too far away, time reseted");
        }
        if (time_left<=0){
            //If he can be saved we destroy him. Since the objective is the civilian to not get eaten so that zombies are not stronger
            //We don't care about points.
            Debug.Log("Civillian saved");
            Destroy(gameObject);
        }
        
        
    }
    Vector3 DodgeZombies(){
        //We will try to get the zombies and masks and try to avoid being more than 30 units close to them
        LayerMask zombieMask = LayerMask.GetMask("Zombie", "Walls");
        Collider[] zombies = Physics.OverlapSphere(transform.position, 30, zombieMask);
        //Zombies collider array also include the close walls
        Vector3 dodgeVector= Vector3.zero;
        foreach(var zombi in zombies){
            Vector3 avoidVector= new Vector3(transform.position.x-zombi.transform.position.x, 0, transform.position.z-zombi.transform.position.z);
            // For each zombie wall we want to calculate the physics vector to save from that exact one (the direction to go to avoid him the fastest way)
            //Debug.Log(transform.position+""+avoidVector+""+zombi.transform.position);\
            dodgeVector=dodgeVector+(avoidVector.normalized/avoidVector.magnitude);
            //The total dodgeVector ( vector to dodge all the obstacles) will have into account the vector to dodge this iteration obstacle
            //We want the closest obstacles to be more important to dodge than the ones further away so we divide by the magnitude.
        }
        //Since the Walls (buildings) have their ceneter further away their dodgeVectors are going to be smaller which is fine
        //As we prefer the civilian to dodge a zombi by coming close to a wall more than a civilian dodge a wall by getting closer to a building
        
        //We return the resulting dodging vector normalized with a times 5 intensity to be really important in comparison with the objective vector (surviving>destination)
        return dodgeVector.normalized * 5;
    }
}
