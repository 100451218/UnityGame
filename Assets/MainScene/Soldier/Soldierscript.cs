using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    void OnEnter();
    void UpdateState();
    void OnExit();
}

public class StateMachine
{
    IState currentState;
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

public class ObserveState : IState
{
    Soldierscript owner;
    //We get the soldier that is calling this state
    public ObserveState(Soldierscript owner) { this.owner = owner;}
    
    GameObject target;
    float target_distance;
    public void OnEnter()
    {
        // "No enemies on my line of sight sir"
    }
    public void UpdateState()
    {
        //Debug.Log("Observe Update");
        if (owner.current_bullets ==0){
            //first check if he has bullets
            owner.statemachine.ChangeState(new ReloadState(owner));
        } else
        {
            //if he has enough bullets we check if there is an enemy in a dangerous range to aim at him
            (target, target_distance)=owner.ClosestEnemy();
            if (target_distance<owner.lookrange){
                //if there is an enemy close enough, he will aim at him
                owner.statemachine.ChangeState(new AimState(owner));
            } else 
            {
                //if there is no thread we can check if we should reload
                if (owner.current_bullets<owner.max_bullets_cappacity){
                        owner.statemachine.ChangeState(new ReloadState(owner));
                }
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
    }
    public void UpdateState()
    {
        //Debug.Log("Aim state update");
        
        // "The enemy is moving, I'll keep watching him"
        (target, target_distance)=owner.ClosestEnemy();
        var rotation = Quaternion.LookRotation(target.transform.position - owner.transform.position);
        owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, rotation, Time.deltaTime * 5);

        target.GetComponent<MeshRenderer>().material = owner.looked;

        if (target_distance<owner.shootrange){
            owner.statemachine.ChangeState(new ShootState(owner));
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
        owner.statemachine.ChangeState(new ObserveState(owner));
    }
    public void OnExit()
    {
        // "Few, that was close"
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
        Debug.Log("Reloading!");
        owner.current_bullets=owner.max_bullets_cappacity;
        owner.statemachine.ChangeState(new ObserveState(owner));
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
    // in game values
    public float lookrange = 1000f;
    public float shootrange = 400f;
    public int max_bullets_cappacity = 5;
    public int current_bullets;
    public Material looked;
    

    // axuiliar variables
    Renderer Enemie_Renderer;
    
    public Vector3 defaultLook;
    void Start(){
        defaultLook= transform.forward;
        statemachine.ChangeState(new ObserveState(this));
        current_bullets = max_bullets_cappacity;
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
            if (currentDistance < distance && (angle>-60 && angle < 60)){
                closest=enemie;
                distance = currentDistance;
            }
        }
        
        return (closest, distance);
    }
    public void Shoot()
    {
        current_bullets --;
        Debug.Log(gameObject.name+current_bullets);
        //transform.LookAt(target.transform);
        //Transform.RotateTowards would be
        RaycastHit hitInfo;
        
        if (Physics.Raycast(this.transform.position, this.transform.forward, out hitInfo, lookrange))
        {
            if (hitInfo.transform.gameObject.tag=="Enemie"){
                
                Destroy(hitInfo.transform.gameObject);     
            }
            // Additional hit effects can be implemented here
        }
    }
}