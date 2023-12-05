using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



/// <summary>
/// Only left that the soldier shoots while it follows and that it actually goes a bit behind (DONE) so that its not all the time colliding.
/// Left to check if I should put the reading of follow and the change states in the Patrol/FollowState instead of the script update
/// No estoy seguro de que termine de funcionar la vuelta al punto mas cercano pero bueno que se le va a hacer
/// 
/// </summary>
public interface IState
{
    void OnEnter();
    void UpdateState();
    void OnExit();
}

public class StateMachine
{
    public IState currentState;
    public void ChangeState(IState newState)
    {
        if (currentState != null){
            currentState.OnExit();
        }
        currentState = newState;
        currentState.OnEnter();
    }
    public void Update(){
        if (currentState != null){
            currentState.UpdateState();
        }
    }
}

public class Model : IState
{
    Soldierscript owner;
    //We get the soldier that is calling this state
    public Model(Soldierscript owner) { this.owner = owner;}

    public void OnEnter(){}
    public void UpdateState(){}
    public void OnExit(){}
}


public class FollowState : IState
{
    Soldierscript owner;
    //We get the soldier that is calling this state
    public FollowState(Soldierscript owner) { this.owner = owner;}

    public void OnEnter(){
        owner.agent.stoppingDistance=2;
        owner.current_state=owner.follow_state;
        owner.agent.destination=GameObject.Find("Commander").transform.position;
        owner.statemachine.ChangeState(owner.observe_state);
    }
    public void UpdateState()
    {
        
    }
    public void OnExit(){
        
    }
}


public class PatrolState : IState
{
    Soldierscript owner;
    //We get the soldier that is calling this state
    public PatrolState(Soldierscript owner) { this.owner = owner;}
    Transform patrol_point;

    bool positioning;
    bool rotating;
    int patrol_counter=0;
    public void OnEnter()
    {
        //Debug.Log("Patrol state entered"+owner.gameObject.name);
        owner.current_state=owner.patrol_state;
        owner.agent.stoppingDistance=0;
        positioning=false;
        rotating= false;
        //first we get all the spawn points
        Transform points = GameObject.Find("CivilianSpawnerControl").transform;
        foreach (Transform point in points)
        {
            if (patrol_point != null){
                if (Vector3.Distance(owner.gameObject.transform.position, patrol_point.position)> Vector3.Distance(owner.gameObject.transform.position, point.position)){
                patrol_point=point;
                }
            } else {
                patrol_point=point;
            } 
        }
        //Debug.Log(patrol_point);
        // Now we have the point the soldier needs to patrol, we can start making him go there
        positioning = true;
        owner.agent.destination= patrol_point.position;
        //Debug.Log("Moving to the point (positioning)");
    }
    public void UpdateState()
    {
        //When we get into this function we need to check if the soldier is moving, if he is we should not do anything else
        if (positioning==true){
            //We just check if the soldier is in the patrol point
            //Debug.Log("positioning"+owner.gameObject.name+owner.gameObject.transform.position+" "+owner.agent.destination);
            if (owner.gameObject.transform.position.x==owner.agent.destination.x && owner.gameObject.transform.position.z==owner.agent.destination.z){positioning=false;}
        }  else if (rotating==true){
            //Debug.Log("Rotating is true, positioning is false, rotating"+ owner.defaultLook+owner.transform.forward);
            //We just need to keep rotating
            var rotation = Quaternion.LookRotation(owner.defaultLook);
            owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, rotation, Time.deltaTime * 5);
            //Debug.Log(owner.transform.forward+ owner.defaultLook);
            if (owner.transform.forward==owner.defaultLook){
                //If we are looking at the street already we can observe it.\
                //Debug.Log("Exit the patrol");
                owner.statemachine.ChangeState(owner.observe_state);
            }
        } else {
            //once we are at our patrol point we need to:
            // Set the default direction and start observing that direction
            //Debug.Log("a");
            //Debug.Log("Rotate is false and postioning also is");
            switch (patrol_counter%4){
                case 0:
                    owner.defaultLook=new Vector3(-1, 0, 0);
                    break;
                case 1:
                    owner.defaultLook=new Vector3(0, 0, 1);
                    break;
                case 2:
                    owner.defaultLook=new Vector3(1, 0, 0);
                    break;
                case 3:
                    owner.defaultLook=new Vector3(0, 0, -1);
                    break;
            }
            
            // Then we start observing that direction
            rotating=true;
            
        }
    }
    public void OnExit()
    {
        //Debug.Log("Exiting patrol");
        positioning=false;
        rotating = false;
        patrol_counter++;
    }
}

public class ObserveState : IState
{
    Soldierscript owner;
    //We get the soldier that is calling this state
    public ObserveState(Soldierscript owner) { this.owner = owner;}
    
    GameObject target;
    float target_distance;
    public void OnEnter()
    {
        //Debug.Log("Observe state "+owner.gameObject.name);
        // "No enemies on my line of sight sir"
        //Debug.Log("b");
    }
    public void UpdateState()
    {
        //Debug.Log("Observe Update");
        if (owner.current_bullets ==0){
            //first check if he has bullets
            //Debug.Log("???");
            owner.statemachine.ChangeState(owner.reload_state);
        } else
        {
            //if he has enough bullets we check if there is an enemy in a dangerous range to aim at him
            (target, target_distance)=owner.ClosestEnemy();
            if (target_distance<owner.lookrange){
                //if there is an enemy close enough, he will aim at him
                //Debug.Log("enemy on sight");
                owner.statemachine.ChangeState(owner.aim_state);
            } else 
            {
                //if there is no threat we need to look another street
                owner.statemachine.ChangeState(owner.current_state);    
            } 
        } 
        
        
    }
    public void OnExit()
    {
        // "I have to do something"
    }
}

public class AimState : IState
{
    Soldierscript owner;
    GameObject target;
    float target_distance;
    //We get the soldier that is calling this state
    public AimState(Soldierscript owner) { this.owner = owner;}
    public void OnEnter()
    {
        // "I see an enemy, I will try to keep it on my sight"
        //Debug.Log("aiming");
    }
    public void UpdateState()
    {
        //Debug.Log("Aim state update");
        
        // "The enemy is moving, I'll keep watching him"
        if (owner.current_bullets<0){
            owner.statemachine.ChangeState(owner.reload_state);
        } else{
            (target, target_distance)=owner.ClosestEnemy();
            var rotation = Quaternion.LookRotation(target.transform.position - owner.transform.position);
            owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, rotation, Time.deltaTime * 10);
            //Debug.Log(target.transform.position);
            target.GetComponent<MeshRenderer>().material = owner.looked;

            if (target_distance<owner.shootrange && Vector3.Angle(target.transform.position-owner.transform.position, owner.transform.forward)<7){
                owner.statemachine.ChangeState(owner.shoot_state);
            }
        }
        
        
    }
    public void OnExit()
    {
        // "I will shoot the enemy or move to the marked position"
    }
}

public class ShootState : IState
{
    Soldierscript owner;
    //We get the soldier that is calling this state
    public ShootState(Soldierscript owner) { this.owner = owner;}
    public void OnEnter()
    {
        // "That enemy got too close! Paw! Paw! Paw!"
    }
    public void UpdateState()
    {
        // "Is he dead now?"
        owner.Shoot();
        //optional add of keep shooting if the target is the same
        owner.statemachine.ChangeState(owner.observe_state);
    }
    public void OnExit()
    {
        // "Few, that was close"
        //owner.current_bullets--;
    }
}


public class ReloadState : IState
{
    Soldierscript owner;
    public ReloadState(Soldierscript owner) { this.owner = owner;}
    public void OnEnter()
    {
        // "Reloading!"
        //ChangeState(observe_state);
        //Debug.Log("Reloading!");
        owner.current_bullets=owner.max_bullets_cappacity;
        owner.statemachine.ChangeState(owner.observe_state);
    }
    public void UpdateState()
    {
        // "Getting out the chamber, getting in the chamber"

    }
    public void OnExit()
    {
        // "locked and loaded"
    }
}

public class MoveState : IState
{
    public void OnEnter()
    {
        // "Changing my position"
    }
    public void UpdateState()
    {
        // "Moving"

    }
    public void OnExit()
    {
        // "I have arrived to my new position"
    }
}


public class Soldierscript : MonoBehaviour
{
    public StateMachine statemachine = new StateMachine();

    public ObserveState observe_state;
    public  AimState aim_state;
    public ShootState shoot_state;
    public ReloadState reload_state;
    public PatrolState patrol_state;
    public FollowState follow_state;



    public float lookrange = 1000f;
    public float shootrange = 400f;
    public int max_bullets_cappacity = 5;
    public NavMeshAgent agent;
    public bool follow=false;

    public int current_bullets;
    public IState current_state;
    public Material looked;
    

    // axuiliar variables
    Renderer Enemie_Renderer;
    
    public Vector3 defaultLook;
    void Start(){
        agent = GetComponent<NavMeshAgent>();
        defaultLook= transform.forward;
        //Debug.Log(defaultLook);

        current_bullets = max_bullets_cappacity;
        observe_state = new ObserveState(this);
        aim_state = new AimState(this);
        shoot_state = new ShootState(this);
        reload_state = new ReloadState(this);
        patrol_state = new PatrolState(this);
        follow_state = new FollowState(this);
        current_state=patrol_state;
        statemachine.ChangeState(patrol_state);
    }
    void Update()
    {   
        statemachine.Update();
        
        /*
        (target, target_distance)=ClosestEnemy();
        if (target_distance<lookrange){
            
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, target.transform.position, 1.0f*Time.deltaTime, 10.0f);
            newDirection.y=0f;
            transform.rotation = Quaternion.LookRotation(newDirection);
            
            //transform.LookAt(target.transform);

            //var rotation = Quaternion.LookRotation(target.transform.position - transform.position);
            //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5);

            target.GetComponent<MeshRenderer>().material = looked;
        } else {
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, defaultLook   , 1.0f*Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
        if (target_distance<shootrange)
        {
        //Too close, shoot
            Shoot();
        }
        */
        //Debug.Log("Follow value: "+follow);
        if (follow==true && current_state!=follow_state){
            //We change the current status
            Debug.Log("banana");
            current_state=follow_state;
            statemachine.ChangeState(follow_state);
            Debug.Log("banana");
            
        } else if (follow==false && current_state!=patrol_state) {
            //If you are not following and you are currently not patrolling
            current_state=patrol_state;
            statemachine.ChangeState(patrol_state);
        }
        
    }





   
    public (GameObject, float) ClosestEnemy(){
        GameObject[] enemies;
        enemies=GameObject.FindGameObjectsWithTag("Enemie");
        GameObject closest=null;
        float distance = 10000000000000;
        //Should be inifinity but it does not work
        Vector3 position = transform.position;
        foreach (GameObject enemie in enemies){
            Vector3 enemie_distance = enemie.transform.position - position;
            float currentDistance = enemie_distance.sqrMagnitude;
            Enemie_Renderer= enemie.GetComponent<Renderer>();
            //We get that enemie renderer
            Vector3 direction = enemie.transform.position- this.transform.position;
            float angle= Vector3.Angle(direction, this.transform.forward);
            //Now we need to check if it is the closest one the soldier sees.
            if (currentDistance < distance && (angle < 120)){
                closest=enemie;
                distance = currentDistance;
            }
        }
        //Debug.Log(closest.transform.position);
        //Debug.Log("papagayo"+closest+distance);
        return (closest, distance);
    }
    public void Shoot()
    {
        current_bullets --;
        //Debug.Log(gameObject.name+current_bullets);
        // now we send the sound to all the zombies nearby
        /*
        //Debug.Log("PAW");
        LayerMask layermask = LayerMask.GetMask("Zombie");
        Collider[] sound_colliders = Physics.OverlapSphere(this.transform.position, 50000, layermask);
        foreach (var collider in sound_colliders){
            //Debug.Log(collider.name);
        }
        */

        //transform.LookAt(target.transform);
        //Transform.RotateTowards would be
        RaycastHit hitInfo;
        
        if (Physics.Raycast(this.transform.position, this.transform.forward, out hitInfo, lookrange))
        {
            Debug.DrawRay(this.transform.position, this.transform.forward*10000, Color.yellow);
            Debug.DrawLine(this.transform.position, this.transform.forward, Color.yellow, 5f);
            //Debug.Log("Shooter position"+this.transform.position+""+hitInfo.transform.gameObject);
            if (hitInfo.transform.gameObject.tag=="Enemie"){
                current_bullets--;
                Destroy(hitInfo.transform.gameObject);     
            }
            // Additional hit effects can be implemented here
        }
    }
}