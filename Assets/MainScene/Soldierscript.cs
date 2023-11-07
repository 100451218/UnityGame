using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public interface IState
{
    void OnEnter();
    void UpdateState();
    void OnExit();
}



public class ObserveState : IState
{
    public void OnEnter()
    {
        // "No enemies on my line of sight sir"
    }
    public void UpdateState()
    {
        // "I will check if I see enemies and reload if needed"
        (target, target_distance)=ClosestEnemy();
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, defaultLook   , 1.0f*Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
        if (target_distance<lookrange){
            ChangeState(aim_state);
        }
        if (current_bullets<max_bullets_cappacity){
            ChangeState(reload_state);
        }
    }
    public void OnExit()
    {
        // "I have to do something"
    }
}

public class AimState : IState
{
    public void OnEnter()
    {
        // "I see an enemy, I will try to keep it on my sight"
    }
    public void UpdateState()
    {
        // "The enemy is moving, I'll keep watching him"
        (target, target_distance)=ClosestEnemy();
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, target.transform.position, 1.0f*Time.deltaTime, 10.0f);
        newDirection.y=0f;
        transform.rotation = Quaternion.LookRotation(newDirection);
        
        //transform.LookAt(target.transform);

        //var rotation = Quaternion.LookRotation(target.transform.position - transform.position);
        //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5);

        target.GetComponent<MeshRenderer>().material = looked;
        if (target_distance<shootrange){
            ChangeState(shoot_state);
        }
    }
    public void OnExit()
    {
        // "I will shoot the enemy or move to the marked position"
    }
}

public class ShootState : IState
{
    public void OnEnter()
    {
        // "That enemy got too close! Paw! Paw! Paw!"
    }
    public void UpdateState()
    {
        // "Is he dead now?"

    }
    public void OnExit()
    {
        // "Few, that was close"
    }
}


public class ReloadState : IState
{
    public void OnEnter()
    {
        // "Reloading!"
        ChangeState(observe_state);
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
    public void ChangeState(IState newState)
    {
        if (currentState!= null){
            currentState.OnExit();
        }
        currentState = newState;
        currentState.OnEnter();
    }
    // state machine variables
    IState currentState;
    public ObserveState observe_state= new ObserveState();
    public AimState aim_state = new AimState();
    public ShootState shoot_state = new ShootState();
    public ReloadState reload_state = new ReloadState();
    public MoveState move_state = new MoveState();

    // in game values
    public float lookrange = 1000f;
    public float shootrange = 400f;
    public int max_bullets_cappacity = 5;
    public Material looked;
    

    // axuiliar variables
    Renderer Enemie_Renderer;
    GameObject target;
    float target_distance;
    private Vector3 defaultLook;
    void Start(){
        defaultLook= transform.forward;
        ChangeState(observe_state);
    }
    void Update()
    {   
        if (currentState!= null){
            currentState.UpdateState();
        }
        
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





   
    (GameObject, float) ClosestEnemy(){
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
    void Shoot()
    {
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
