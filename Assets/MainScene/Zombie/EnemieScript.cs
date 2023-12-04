using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IDLEState : IState
{
    EnemieScript owner;
    public IDLEState(EnemieScript owner) { this.owner = owner;}
    float timer=0;
    public void OnEnter()
    {
        owner.gameObject.GetComponent<MeshRenderer>().material= owner.idle_mat;
    }
    public void UpdateState()
    {
        //Debug.Log("IDLE");
        timer = timer+Time.deltaTime;
        if (timer>6){
            //Time to move to a different random position
            float x= owner.transform.position.x+ Random.Range(-20, 20);
            float z= owner.transform.position.z+ Random.Range(-20, 20);
            owner.agent.destination= new Vector3(x,0,z);
            timer = 0;
        }
        if (owner.Detect()){
            //If the zombie has detected something he will follow it so we switch to chase state
            owner.statemachine.ChangeState(new ChaseState(owner));
        }
    }
    public void OnExit(){}
}

public class ChaseState : IState
{
    
    EnemieScript owner;
    public ChaseState(EnemieScript owner) { this.owner = owner;}
    public void OnEnter()
    {
        owner.gameObject.GetComponent<MeshRenderer>().material= owner.chasing_mat;
    }
    public void UpdateState()
    {
        //Debug.Log("Chasing! "+ owner.objective+owner.objective.name);
        //braaaaiiiiinnnzzzzzs
        //The zombie will chase his objective and therefore we need to make it so that he follows the enemie he saw until he looses the line of sight (then he will investigate last known position)
        //We can assume that navmesh make it look towards the objective, therefore we just need to make sure he can see him
        RaycastHit hitInfo;
        
        if (!owner.objective){
            //Debug.Log("ded");
            owner.statemachine.ChangeState(new IDLEState(owner));
        } else {
            Vector3 direction = owner.objective.transform.position-owner.transform.position;
            //We need to check if the objective is that close that they are within attack range (colliders don't work)
            if (direction.magnitude<2){
                Debug.Log("atacked");
            }
            if (Physics.Raycast(owner.transform.position, direction, out hitInfo, owner.visionrange))
        {
            //Debug.Log(hitInfo.transform.gameObject);
            if (hitInfo.transform.gameObject.tag=="Buildings"){
                //He has stoped seeing the enemy
                owner.statemachine.ChangeState(new InvestigateState(owner)); 
            } else {
                //if he can see him still he will move accordingly
                owner.agent.destination= owner.objective.transform.position;
            }
            // If he is still can see the enemie then we need to maintain the target
        }
        }
        
        
    }
    public void OnExit(){}
}


public class InvestigateState : IState
{
    EnemieScript owner;
    public InvestigateState(EnemieScript owner) { this.owner = owner;}
    public void OnEnter()
    {
        //The objective should still be the same, go to the last point the zombie saw the commander
        owner.gameObject.GetComponent<MeshRenderer>().material= owner.investigate_mat;
    }
    public void UpdateState()
    {
        //We won't keep updating the zombie target position as the last one seen was the one he is currently going at
        //Because he no longer has an actual objective to chase, if he sees/hears something he will go there instead
        if (owner.Detect()){
            //if the zombie detects another enemie (sees him) he will chase that enemie rather than investigating
            owner.statemachine.ChangeState(new ChaseState(owner));
        } /*else if (owner.Hear())
        {
            //If the enemy does not see anything he will try to hear something
            // he will always go to the closest sound 
            // first we calculate how far is the thing he is investigating

            // second we calculate 
        }
        */
    }
    public void OnExit(){}
}


public class EnemieScript : MonoBehaviour
{
    public int visionrange = 10000;
    public Material chasing_mat;
    public Material idle_mat;
    public Material investigate_mat;
    public StateMachine statemachine = new StateMachine();
    GameObject commander;
    public NavMeshAgent agent;
    public GameObject objective;
    GameObject[] civilians;
    // Start is called before the first frame update
    void Start()
    {
        //Only if he hears something
        commander = GameObject.FindWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        statemachine.ChangeState(new IDLEState(this));
        //agent.destination= commander.transform.position;
    }
    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Entered collision with " + collision.gameObject.name);
        if (collision.gameObject.tag=="Civilian"){
            Destroy(collision.gameObject);
            agent.speed=agent.speed*2;
        } else if (collision.gameObject.name=="Commander"){
            Debug.LogError("Game Over");
        }

    }

    // Update is called once per frame
    void Update()
    {
        statemachine.Update();  
    }
    public bool Detect(){
        //This code will detect if the zombie should chase another enemy
        // We need to check if the zombi sees a commander or a civilian
        float distance= 10000000000000000000;
        GameObject target_civilian = null;
        //The zombi will go to the closest civilian he sees
        civilians=GameObject.FindGameObjectsWithTag("Civilian");
        if (civilians!= null){
            foreach(GameObject civilian in civilians){
                Vector3 direction =civilian.transform.position - gameObject.transform.position;
                float angle= Vector3.Angle(direction, gameObject.transform.forward);
                if (angle < 45){
                    //if he sees him
                    Vector3 enemie_distance = civilian.transform.position - gameObject.transform.position;
                    float currentDistance = enemie_distance.sqrMagnitude;
                    if (currentDistance<distance && currentDistance<15000){
                        target_civilian= civilian;
                        distance=currentDistance;
                    }           
                }
            }
        }
        if (target_civilian != null){
            agent.destination= target_civilian.transform.position;
            objective= target_civilian;
            return true;
        } else{
            //if he does not see any civilian, check if he sees the commander
            Vector3 direction =commander.transform.position - gameObject.transform.position;
            float angle= Vector3.Angle(direction, gameObject.transform.forward);
            if (angle < 45){
                //if he is in the angle of seeing him
                RaycastHit hitInfo;
                //direction = commander.transform.position-gameObject.transform.position;
                if (Physics.Raycast(this.transform.position, direction, out hitInfo, 100000))
                {  
                    //Debug.Log("Checking Dectect "+ hitInfo.transform.gameObject);
                    //Check if it collides with a building
                    if (hitInfo.transform.gameObject.tag=="Player"){
                        //If the raycast sees the commander, then the commander is the objective
                        agent.destination = commander.transform.position;
                        objective= commander;
                        return true;
                    }
                }        
            }
            //if there is not civilian he sees or the commander is not at sight, simply do nothing
        }
        return false;
    }

    public void Investigate(Vector3 position){
        Debug.Log("Zombi heard something");
    }
    public void Chase(NavMeshAgent chased){
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.destination= commander.transform.position;
    }
}
