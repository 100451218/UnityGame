using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


/// <summary>
/// Currently the commander doesn't have into account he can't run into walls, xd

/// </summary>
public class CommanderScript : MonoBehaviour
{   
    public NavMeshAgent agent;
    private Vector3 currentVelocity;


    public Camera Commander;
    public Camera Camera1;
    public Camera Camera2;
    public Camera Camera3;
    
    
    public GameObject move_pointer;
    

    // Use this for initialization
    void Start()
    {   
        agent = GetComponent<NavMeshAgent>();
        Camera3.enabled = false;
        Commander.enabled= true;
        Camera1.enabled = false;
        Camera2.enabled = false;
       
    }
    public void SwitchMainCamera(){
        Camera3.enabled = false;
        Commander.enabled= true;
        Camera1.enabled = false;
        Camera2.enabled = false;
    }
    public void SwitchTimCamera(){
        Camera3.enabled = true;
        Commander.enabled= false;
        Camera1.enabled = false;
        Camera2.enabled = false;
    }
    public void SwitchBobCamera(){
        Camera3.enabled = false;
        Commander.enabled= false;
        Camera1.enabled = true;
        Camera2.enabled = false;
    }
    public void SwitchSamCamera(){
        Camera3.enabled = false;
        Commander.enabled= false;
        Camera1.enabled = false;
        Camera2.enabled = true;
    }
    public void SwitchUAVCamera(){
        Camera3.enabled = false;
        Commander.enabled= false;
        Camera1.enabled = false;
        Camera2.enabled = false;
    }

    public void MoveSoldier(string soldier_name){
        //get the soldier and make him change his primary state Patrol/Follow
        GameObject soldier = GameObject.Find(soldier_name);
        Debug.Log(soldier.name+soldier.GetComponent<Soldierscript>().follow);
        soldier.GetComponent<Soldierscript>().follow=!soldier.GetComponent<Soldierscript>().follow;
        Debug.Log(soldier.name+soldier.GetComponent<Soldierscript>().follow);
    }



    public void Move()
    {
        
        RaycastHit hitInfo;
        Camera observing_point=Camera.main;
        
        if (Physics.Raycast(observing_point.transform.position, observing_point.transform.forward, out hitInfo, 10000))
        {   
            //We do a raycast with the active camera
            Debug.Log(hitInfo.transform.name + hitInfo.transform.position);
            var localHit = transform.InverseTransformPoint(hitInfo.point);
            
            if (hitInfo.transform.name=="MinimapCanvas"){
                //if it is the minimap, we need to change the point to an actual point
                Vector3 canvas_transform = hitInfo.transform.InverseTransformPoint(hitInfo.point);
                //Now we have the local position the user wants to go, we need to inverse that position to the real worlds
                Vector2 local_position= new Vector2(canvas_transform.x, canvas_transform.z);
                
                Transform origin = GameObject.Find("origin").transform;
                Transform final = GameObject.Find("final").transform;
                Vector3 distance_relative = origin.InverseTransformPoint(final.position);
                
                float x_ratio= Mathf.Abs((300)/(distance_relative.x));
                local_position=local_position/x_ratio;
                //local_position would be the quivalent of distance relative in the Pointers.cs code, it is the distance of the real point to the origin
                Vector3 real_position = origin.TransformPoint(local_position);
                real_position= new Vector3(real_position.x+150f, real_position.y-1f, real_position.z-150);
                Debug.Log("final position "+ real_position);

                agent.destination=real_position;
                
               
            } else {
                //If we hit other thing we move that way
                agent.destination=hitInfo.point;
            }
            
            
        }
    }
    

    Vector3 DodgeZombies(){
        LayerMask zombieMask = LayerMask.GetMask("Zombie", "Walls");
        Collider[] zombies = Physics.OverlapSphere(transform.position, 30, zombieMask);
        Vector3 dodgeVector= Vector3.zero;
        foreach(var zombi in zombies){
            Vector3 avoidVector= transform.position-zombi.transform.position;
            dodgeVector=dodgeVector+(avoidVector.normalized/avoidVector.magnitude);
        }
        return dodgeVector.normalized * 5;
    }
    // Update is called once per frame
    void Update()
    {
        //Ai part to prevent the commander to be eated when not moving
        Vector3 desiredV = Vector3.zero;
        if (agent.hasPath){
            desiredV = agent.desiredVelocity;
        } 

        Vector3 survivingV = DodgeZombies();
        desiredV= desiredV+survivingV;
        currentVelocity = Vector3.Lerp(currentVelocity, desiredV, Time.deltaTime);
        agent.Move(currentVelocity * Time.deltaTime);
        
        if (Input.GetKeyDown("1"))
        {
            Commander.enabled = true;
            Camera1.enabled = false;
            Camera2.enabled = false;
            Camera3.enabled = false;
        }
        if (Input.GetKeyDown("2"))
        {
            Commander.enabled = false;
            Camera1.enabled = true;
            Camera2.enabled = false;
            Camera3.enabled = false;
        }
        if (Input.GetKeyDown("3"))
        {
            Commander.enabled = false;
            Camera1.enabled = false;
            Camera2.enabled = true;
            Camera3.enabled = false;
        }
        if (Input.GetKeyDown("4"))
        {
            Commander.enabled = false;
            Camera1.enabled = false;
            Camera2.enabled = false;
            Camera3.enabled = true;
        }
    }

}