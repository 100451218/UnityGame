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


public class reloadState : IState
{
    public void OnEnter()
    {
        // "Reloading!"
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


    IState currentState;


    public float lookrange = 1000f;
    public float shootrange = 400f;
    public Material looked;
    Renderer Enemie_Renderer;
    GameObject target;
    float target_distance;
    private Vector3 defaultLook;
    void Start(){
        defaultLook= transform.forward;
    }
    void Update()
    {
        currentState.UpdateState();

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
        
    }





    public void ChangeState(IState newState)
    {
        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter();
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
