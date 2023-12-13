using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public interface IState
{
    //We define the interface structure to the soldiers states
    void OnEnter();
    void UpdateState();
    void OnExit();
}

public class StateMachine
{
    //We have to create the class of the state machine
    public IState currentState;
    public void ChangeState(IState newState)
    {
        //Whenever we change a state to a new one, we need to do the old state OnExit action
        if (currentState != null){
            currentState.OnExit();
        }
        //We define the current state as the new state and execute that new state OnEnter action
        currentState = newState;
        currentState.OnEnter();
    }
    public void Update(){
        //On each frame we will do the OnUpdate action of the current state
        if (currentState != null){
            currentState.UpdateState();
        }
    }
}

public class Model : IState
{
    //Class representing the essential structure of the states of all the NPCs
    Soldierscript owner;
    //We get the soldier (or zombie) that is calling this state
    public Model(Soldierscript owner) { this.owner = owner;}
    //We define the variable owner as the Soldier script so that we can access calling object local variables.
    public void OnEnter(){
        //This function will only be executed when we get to the state
    }
    public void UpdateState(){
        //This function will be executed periodically (every frame)
    }
    public void OnExit(){
        //This function will be executed before we go to another state
    }
}


public class FollowState : IState
{
    /// In this state the soldier will be following the commander arround
    Soldierscript owner;
    //We get the soldier that is calling this state
    public FollowState(Soldierscript owner) { this.owner = owner;}

    public void OnEnter(){
        //We need to set that the soldier stops two units before getting to the commander position, to prevent the turret from pushing him
        owner.agent.stoppingDistance=2;
        owner.current_state=owner.follow_state;
        //We set the variable current_state as the follow state. This is done as some states are what would be considered "complimentary states"
        // The two main states are Follow and Patrol, we need to know which one the turret is in so that it can go back after doing complimentary states like shooting
        owner.agent.destination=GameObject.Find("Commander").transform.position;
        //We make the soldier to move to the commander position (stopping 2 units before)
        owner.statemachine.ChangeState(owner.observe_state);
        //Now that the navmesh is going to make the turret move, we can have it observing its surrondings
    }
    public void UpdateState()
    {
        
    }
    public void OnExit(){
        
    }
}


public class PatrolState : IState
{
    //In this state the soldier will be standing still on position and look arround for enemies.
    //We need to first go to our patrol point, look at the next observing position and then observe
    //The observing position are the 4 cardinal directions (NSEW)
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
        //The main state now is Patrol
        owner.current_state=owner.patrol_state;
        //We no longer need the soldier to stop 2 units before its destination
        owner.agent.stoppingDistance=0;
        rotating= false;
        
    }
    public void UpdateState()
    {
        //When we get into this function we need to check if the soldier is rotating to his new observing direction, if he is we should not do anything else
        //If he is not it means we need to calculate the new direction to look at
        if (rotating==true){
            //We just need to keep rotating
            var rotation = Quaternion.LookRotation(owner.defaultLook);
            owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, rotation, Time.deltaTime * 5);
            //Debug.Log(owner.transform.forward+ owner.defaultLook);
            if (owner.transform.forward==owner.defaultLook){
                //If we are looking at the direction already we can observe it.\
                //Debug.Log("Exit the patrol");
                owner.statemachine.ChangeState(owner.observe_state);
            }
        } else {
            // We need to set the default direction (observing direction) and start observing that direction (in the code block above)
            //To know which direction we wants we simply can have a counter.
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
            
            // Then we start rotating towards that direction
            rotating=true;
            
        }
    }
    public void OnExit()
    {
        //Debug.Log("Exiting patrol");
        rotating = false;
        patrol_counter++;
        //So that next iteratio we look another way
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
        
    }
    public void UpdateState()
    {
        //Debug.Log("Observe Update");
        if (owner.current_bullets ==0){
            //first check if he has bullets
            //If not, we need to reload
            owner.statemachine.ChangeState(owner.reload_state);
        } else
        {
            //if he has enough bullets to shoot once we check if there is an enemy in a dangerous range to aim at him
            (target, target_distance)=owner.ClosestEnemy();
            if (target_distance<owner.lookrange){
                //if there is an enemy close enough, he will aim at him
                //Debug.Log("enemy on sight");
                owner.statemachine.ChangeState(owner.aim_state);
            } else 
            {
                //if there is no threat we need to do what our main state says.
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
        //The soldier will try to aim at the enemy but first we need to double check he has bullets
        if (owner.current_bullets<0){
            owner.statemachine.ChangeState(owner.reload_state);
        } else{
            //If there are bullets we get the closest enemy on sight and his distance
            (target, target_distance)=owner.ClosestEnemy();
            var rotation = Quaternion.LookRotation(target.transform.position - owner.transform.position);
            //We get the rotation the soldier needs to do so that he is looking directly at the enemy
            owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, rotation, Time.deltaTime * 10);
            //Debug.Log(target.transform.position);
            //We change the zombie material, with the new model it won't be appreciated but is still useful for debuging and testing
            target.GetComponent<MeshRenderer>().material = owner.looked;

            if (target_distance<owner.shootrange && Vector3.Angle(target.transform.position-owner.transform.position, owner.transform.forward)<7){
                //If the enemy is close enough and the objective direction and the current direction is less than 7 degrees we can shoot
                //This is done as the new zombie model had an issue where the actual front looking shoot wouldn't hit him.
                //Sometimes the turret needs two shots to kill one target but I decided to leave it to make the game more realistic
                //If I wanted it to be removed I could simply just make the enemy be destroyed under shoot action (if it is seen) instead of casting raycast
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
        // We call the function to shoot and then we go back observing
        owner.Shoot();
        //optional add of keep shooting if the target is the same
        owner.statemachine.ChangeState(owner.observe_state);
    }
    public void UpdateState()
    {
        //No update state
    }
    public void OnExit()
    {
        
    }
}


public class ReloadState : IState
{
    Soldierscript owner;
    public ReloadState(Soldierscript owner) { this.owner = owner;}
    private float counter= 0f;
    public void OnEnter()
    {
        //As we are dropping the mag (or opening the particle acceleration containers) we won't have any more bullets in the mag
        owner.current_bullets=0;   
    }
    public void UpdateState()
    {
        // "Getting out the chamber, getting in the chamber"
        if (counter <1){
            //To reload there is a 1 second duration so we won't be able to do anything while we are reloading
            counter = counter + Time.deltaTime;
        } else{
            //Once reloaded we can go back to the observing state
            owner.statemachine.ChangeState(owner.observe_state);
        }
    }
    public void OnExit()
    {
        // "locked and loaded". We will have now a new loaded mag 
        owner.current_bullets=owner.max_bullets_cappacity;
    }
}



public class Soldierscript : MonoBehaviour
{
    public StateMachine statemachine = new StateMachine();

    public LayerMask ignoreRaycast;
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
    

    
    Renderer Enemie_Renderer;
    
    public Vector3 defaultLook;
    void Start(){
        //We load the NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        //We save where the soldier is looking
        defaultLook= transform.forward;
        //We set the ammount of bullets to the mag capacity
        current_bullets = max_bullets_cappacity;
        //We create all the states and set the initial one to patrol state
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
        //We do the Update action of the current state
        statemachine.Update();
        //We need to also check if the follow boolean variable changes
        //If it is true but we are not in the following state( not following) it means we were patrolling and the follow command came
        if (follow==true && current_state!=follow_state){
            //We change the current status
            current_state=follow_state;
            statemachine.ChangeState(follow_state); 
        } else if (follow==false && current_state!=patrol_state) {
            //If you should not be following and you are currently not patrolling ( so you are following)
            current_state=patrol_state;
            statemachine.ChangeState(patrol_state);
        }
        
    }





   
    public (GameObject, float) ClosestEnemy(){
        //We will get all the enemies and calculate the closest one
        GameObject[] enemies;
        enemies=GameObject.FindGameObjectsWithTag("Enemie");
        GameObject closest=null;
        float distance = 10000000000000;
        //Should be infinity but the value causes issues so we are using an arbitrary large value
        Vector3 position = transform.position;
        foreach (GameObject enemie in enemies){
            //For each enemy, we calculate the distance vector between the turret and him
            Vector3 enemie_distance = enemie.transform.position - position;
            //The value of the distance will be the vector's magnitude
            float currentDistance = enemie_distance.sqrMagnitude;
            Enemie_Renderer= enemie.GetComponent<Renderer>();
            //We get that enemie renderer
            Vector3 direction = enemie.transform.position- this.transform.position;
            float angle= Vector3.Angle(enemie_distance, this.transform.forward);
            //Now we need to check if it is the closest one the soldier sees.
            if (currentDistance < distance && (angle < 120)){
                //If the enemie is closer than the previously closest one and the turret can see him then that enemie is the new closest one
                closest=enemie;
                distance = currentDistance;
            }
        }
        //We return the closest enemy and the distance from him
        return (closest, distance);
    }
    public void Shoot()
    {
        current_bullets --;
        //First we loose a bullet regardless if we hit him or not
        RaycastHit hitInfo;
        
        if (Physics.Raycast(this.transform.position, this.transform.forward, out hitInfo, lookrange, ~ignoreRaycast))
        {
            //We cast a raycast in front of the turret
            //We dray the shooting ray for debugging purposes 
            Debug.DrawRay(this.transform.position, this.transform.forward*10000, Color.yellow);
            Debug.DrawLine(this.transform.position, this.transform.forward, Color.yellow, 5f);
            //If the raycast hits an enemy we destroy him (its killed)
            if (hitInfo.transform.gameObject.tag=="Enemie"){
                Destroy(hitInfo.transform.gameObject);     
            }
            // Additional hit effects can be implemented here like making a bullet hole in a building or killing a civilian if the soldier misses
        }
    }
}